using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
public record AddTasksRequest(List<NewTaskInput>? Tasks);

public record AddTasksCommand(Guid UserId, Guid JobId, AddTasksRequest Request) : IRequest<Result<AddTasksResponse>>;
#endregion

#region --- VALIDATOR ---
public class AddTasksCommandValidator : AbstractValidator<AddTasksCommand>
{
    public AddTasksCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
        RuleFor(v => v.Request.Tasks).NotEmpty().WithMessage("Add at least one task.");
        RuleForEach(v => v.Request.Tasks).SetValidator(new NewTaskInputValidator());
    }
}
#endregion

#region --- HANDLER ---
internal sealed class AddTasksCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    JobTaskPlanner taskPlanner,
    JobNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<AddTasksCommand, Result<AddTasksResponse>>
{
    public async Task<Result<AddTasksResponse>> Handle(AddTasksCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var today = TrackingClock.Today();
        var job = await jobRepository.GetJobAsync(viewer, command.JobId, today, cancellationToken);
        if (job is null) return new RecordNotFoundError("Job not found");
        if (!JobPermissionRules.For(viewer, job).CanUpdate)
        {
            return new ForbiddenError("Only the person who created this job, someone above them, or a manager above the assignee can add tasks.");
        }

        var status = Enum.Parse<JobStatus>(job.Summary.Status);
        if (status is not (JobStatus.Assigned or JobStatus.InProgress or JobStatus.Delayed))
        {
            return new ValidationError($"{job.Summary.JobCode} is {status}; tasks can no longer be added.");
        }

        // The same rules as creating a job, checked against the job's assignee
        var assignee = await scopeRepository.GetOfficerAsync(job.AssignTo, cancellationToken);
        if (assignee is null) return new ValidationError($"{job.Summary.JobCode} has no one holding it, so tasks cannot be added.");

        var now = DateTime.Now;
        var plan = await taskPlanner.PlanAsync(viewer, assignee, job.Summary.Id, job.Summary.StartDate, job.Summary.EndDate ?? today,
            command.Request.Tasks!, job.Summary.Id, now, today, cancellationToken);
        if (plan.Errors.Count > 0) return new ValidationError(string.Join(" ", plan.Errors));
        if (plan.Tasks.Count == 0)
        {
            return new ValidationError($"No task could be added: {string.Join("; ", plan.Skipped.Select(s => $"{s.Title} — {s.Reason}"))}");
        }

        await numberGenerator.AssignTaskNosAsync(plan.Tasks, plan.Branches, today, cancellationToken);
        await jobRepository.AddTasksAsync(plan.Tasks, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await jobRepository.GetJobAsync(viewer, job.Summary.Id, today, cancellationToken);
        return Result.Ok(new AddTasksResponse(JobDocResponse.From(viewer, updated!), plan.Skipped, plan.Warnings));
    }
}
#endregion
