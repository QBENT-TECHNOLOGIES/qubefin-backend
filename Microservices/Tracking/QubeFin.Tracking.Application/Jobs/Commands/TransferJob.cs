using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
// AssigneeId is the new person's designation id. TaskIds null = the whole job; a list = only those pending tasks. EndDate is the new job's date for a partial transfer (null = the original's).
public record TransferJobRequest(Guid? AssigneeId, List<Guid>? TaskIds, string? Remarks, DateOnly? EndDate);

public record TransferJobCommand(Guid UserId, Guid JobId, TransferJobRequest Request) : IRequest<Result<TransferJobResponse>>;

public record TransferJobResponse(JobDocResponse Job, JobDocResponse? NewJob, List<SkippedTask> Skipped);
#endregion

#region --- VALIDATOR ---
public class TransferJobCommandValidator : AbstractValidator<TransferJobCommand>
{
    public TransferJobCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
        RuleFor(v => v.Request.AssigneeId).NotEmpty().WithMessage("Select who will do this job.");
        RuleFor(v => v.Request.Remarks)
            .Must(r => r is not null && r.Trim().Length >= 3).WithMessage("A reason is required for a transfer.");
        RuleFor(v => v.Request.EndDate)
            .Must(d => d >= TrackingClock.Today()).WithMessage("The complete-by date cannot be in the past.")
            .When(v => v.Request.EndDate.HasValue);
    }
}
#endregion

#region --- HANDLER ---
// Completed tasks never move (route and travel allowance depend on them). Nothing done and every pending task moving = the job
// changes hands; otherwise the moving tasks are copied into a new job for the new person and the originals become Transferred.
internal sealed class TransferJobCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    JobNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<TransferJobCommand, Result<TransferJobResponse>>
{
    public async Task<Result<TransferJobResponse>> Handle(TransferJobCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var today = TrackingClock.Today();
        var detail = await jobRepository.GetJobAsync(viewer, command.JobId, today, cancellationToken);
        if (detail is null) return new RecordNotFoundError("Job not found");
        if (!JobPermissionRules.For(viewer, detail).CanTransfer)
        {
            return new ForbiddenError("Only the person who created this job, or someone above them, can do this.");
        }

        var request = command.Request;
        var newAssignee = await scopeRepository.GetOfficerAsync(request.AssigneeId!.Value, cancellationToken);
        if (newAssignee is null) return new ValidationError("Select who will do this job.");
        if (newAssignee.DesignationId == detail.AssignTo || newAssignee.EmployeeId == detail.Summary.AssigneeId)
        {
            return new ValidationError($"{newAssignee.Name} already holds this job.");
        }
        if (!JobPermissionRules.CanGiveWork(viewer, newAssignee)) return new ForbiddenError($"You cannot give work to {newAssignee.Name}.");

        var job = (await jobRepository.GetJobForUpdateAsync(command.JobId, cancellationToken))!;
        var pendingTasks = job.TblJobTasks.Where(JobRepository.IsPending).ToList();
        var doneCount = job.TblJobTasks.Count(t => JobRepository.StatusOf(t) == nameof(JobTaskStatus.Completed));

        List<TblJobTask> selected;
        if (request.TaskIds is null)
        {
            selected = pendingTasks;
        }
        else
        {
            selected = [];
            foreach (var taskId in request.TaskIds.Distinct())
            {
                var task = job.TblJobTasks.FirstOrDefault(t => t.Id == taskId);
                if (task is null) return new RecordNotFoundError("Task not found");
                if (!JobRepository.IsPending(task)) return new ValidationError($"{task.TaskNo} is {JobRepository.StatusOf(task)}.");
                selected.Add(task);
            }
        }
        if (selected.Count == 0) return new ValidationError("There are no pending tasks to transfer.");

        var branches = await scopeRepository.GetBranchCodesAsync(selected.Select(t => t.BranchId), cancellationToken);

        // D3: the new person must work in every branch the moving tasks are in
        var branchErrors = selected
            .Where(t => !newAssignee.BranchIds.Contains(t.BranchId))
            .Select(t => $"{t.TaskTitle}: {newAssignee.Name} does not work in {branches[t.BranchId].Name}.")
            .ToList();
        if (branchErrors.Count > 0) return new ValidationError(string.Join(" ", branchErrors));

        // D5: tasks the new person already has pending at the same place stay in this job
        var windowStart = job.StartDate > today ? job.StartDate : today;
        var windowEnd = request.EndDate ?? job.EndDate ?? today;
        var pendingElsewhere = await jobRepository.GetPendingTasksAtAsync(selected.Where(t => t.OriginRefId.HasValue).Select(t => t.OriginRefId!.Value), cancellationToken);
        var skipped = new List<SkippedTask>();
        var moving = new List<TblJobTask>();
        foreach (var task in selected)
        {
            var duplicate = task.OriginRefId.HasValue
                ? pendingElsewhere.FirstOrDefault(p => p.OriginRefId == task.OriginRefId
                    && p.AssignTo == newAssignee.DesignationId
                    && p.TaskType == task.TaskType
                    && (task.TaskType == nameof(TaskType.OdFollowUp) || Overlaps(p.StartDate, p.EndDate, windowStart, windowEnd, today)))
                : null;
            if (duplicate is not null)
            {
                skipped.Add(new SkippedTask(task.TaskTitle ?? task.TaskNo, $"Already pending with {newAssignee.Name} in {duplicate.JobCode}"));
                continue;
            }
            moving.Add(task);
        }
        if (moving.Count == 0)
        {
            return new ValidationError($"Nothing to transfer: {newAssignee.Name} already has these tasks pending ({string.Join(", ", skipped.Select(s => s.Title))}).");
        }

        var now = DateTime.Now;
        Guid? newJobId = null;
        var isWholeTransfer = doneCount == 0 && moving.Count == pendingTasks.Count;
        if (isWholeTransfer)
        {
            job.AssignTo = newAssignee.DesignationId;
            foreach (var task in moving)
            {
                // The new person cannot also be the one going along (D9)
                if (task.CoOrdinatedBy == newAssignee.DesignationId) task.CoOrdinatedBy = null;
                task.ModifiedOn = now;
            }
        }
        else
        {
            var newJob = new TblJob
            {
                Id = Guid.NewGuid(),
                JobCode = await numberGenerator.NextJobCodeAsync(today, cancellationToken),
                JobTitle = job.JobTitle,
                JobDescription = job.JobDescription,
                AssignTo = newAssignee.DesignationId,
                StartDate = windowStart,
                EndDate = windowEnd,
                TransferJobRefId = job.Id,
                CreatedBy = viewer.UserId,
                CreatedOn = now
            };
            foreach (var task in moving)
            {
                var copy = new TblJobTask
                {
                    Id = Guid.NewGuid(),
                    TaskNo = string.Empty,
                    JobId = newJob.Id,
                    TaskType = task.TaskType,
                    BranchId = task.BranchId,
                    OriginRefId = task.OriginRefId,
                    TaskTitle = task.TaskTitle,
                    Notes = task.Notes,
                    Priority = task.Priority,
                    CoOrdinatedBy = task.CoOrdinatedBy == newAssignee.DesignationId ? null : task.CoOrdinatedBy,
                    TargetName = task.TargetName,
                    TargetLocation = task.TargetLocation,
                    RadiusMeters = task.RadiusMeters,
                    Status = nameof(JobTaskStatus.Pending),
                    IsSelfAdded = task.IsSelfAdded,
                    TaskAddedBy = viewer.UserId,
                    TaskAddedOn = now
                };
                newJob.TblJobTasks.Add(copy);
                task.Status = nameof(JobTaskStatus.Transferred);
                task.TransferTaskId = copy.Id;
                task.ModifiedOn = now;
            }
            await numberGenerator.AssignTaskNosAsync(newJob.TblJobTasks, branches, today, cancellationToken);
            await jobRepository.AddAsync(newJob, cancellationToken);
            newJobId = newJob.Id;
        }
        job.ModifiedBy = viewer.UserId;
        job.ModifiedOn = now;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Both jobs stay visible: the new person is someone this user may give work to, so they are inside the user's scope
        var original = await jobRepository.GetJobAsync(viewer, job.Id, today, cancellationToken);
        var created = newJobId.HasValue ? await jobRepository.GetJobAsync(viewer, newJobId.Value, today, cancellationToken) : null;
        return Result.Ok(new TransferJobResponse(
            JobDocResponse.From(viewer, original!),
            created is null ? null : JobDocResponse.From(viewer, created),
            skipped));
    }

    // A job is active from its start date to its end date; a delayed job stays active until today
    private static bool Overlaps(DateOnly start, DateOnly? end, DateOnly windowStart, DateOnly windowEnd, DateOnly today)
    {
        var activeUntil = end.HasValue && end.Value > today ? end.Value : today;
        return start <= windowEnd && activeUntil >= windowStart;
    }
}
#endregion
