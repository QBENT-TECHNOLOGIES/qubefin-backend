using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
public record CreateJobCommand(Guid UserId, CreateJobRequest Request) : IRequest<Result<AddTasksResponse>>;
#endregion

#region --- VALIDATOR ---
public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request.AssigneeId).NotEmpty().WithMessage("Select who will do this job.");
        RuleFor(v => v.Request.JobTitle).NotEmpty().WithMessage("Job title is required.");
        RuleFor(v => v.Request.JobTitle).MaximumLength(50).WithMessage("Job title can be at most 50 characters.");
        RuleFor(v => v.Request.JobDescription).MaximumLength(500).WithMessage("Description can be at most 500 characters.");
        RuleFor(v => v.Request.EndDate).NotNull().WithMessage("Select the complete-by date.");
        RuleFor(v => v.Request.EndDate)
            .Must(d => d >= TrackingClock.Today()).WithMessage("The complete-by date cannot be in the past.")
            .When(v => v.Request.EndDate.HasValue);
        RuleFor(v => v.Request.StartDate)
            .Must((v, start) => start <= v.Request.EndDate).WithMessage("Start date must be on or before the complete-by date.")
            .When(v => v.Request.StartDate.HasValue && v.Request.EndDate.HasValue);
        RuleFor(v => v.Request.Tasks).NotEmpty().WithMessage("A job needs at least one task.");
        RuleForEach(v => v.Request.Tasks).SetValidator(new NewTaskInputValidator());
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CreateJobCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    JobTaskPlanner taskPlanner,
    JobNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateJobCommand, Result<AddTasksResponse>>
{
    public async Task<Result<AddTasksResponse>> Handle(CreateJobCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var request = command.Request;
        var assignee = await scopeRepository.GetOfficerAsync(request.AssigneeId!.Value, cancellationToken);
        if (assignee is null) return new ValidationError("Select who will do this job.");
        if (!CanGiveWork(viewer, assignee)) return new ForbiddenError($"You cannot give work to {assignee.Name}.");

        var now = DateTime.Now;
        var today = TrackingClock.Today();
        var jobId = Guid.NewGuid();
        var startDate = request.StartDate ?? today;
        var endDate = request.EndDate!.Value;

        var plan = await taskPlanner.PlanAsync(viewer, assignee, jobId, startDate, endDate, request.Tasks!, null, now, today, cancellationToken);
        if (plan.Errors.Count > 0) return new ValidationError(string.Join(" ", plan.Errors));
        if (plan.Tasks.Count == 0)
        {
            return new ValidationError($"No task could be added: {string.Join("; ", plan.Skipped.Select(s => $"{s.Title} — {s.Reason}"))}");
        }

        await numberGenerator.AssignTaskNosAsync(plan.Tasks, plan.Branches, today, cancellationToken);
        var job = new TblJob
        {
            Id = jobId,
            JobCode = await numberGenerator.NextJobCodeAsync(today, cancellationToken),
            JobTitle = request.JobTitle!.Trim(),
            JobDescription = string.IsNullOrWhiteSpace(request.JobDescription) ? null : request.JobDescription.Trim(),
            AssignTo = assignee.DesignationId,
            StartDate = startDate,
            EndDate = endDate,
            CreatedBy = viewer.UserId,
            CreatedOn = now,
            TblJobTasks = plan.Tasks
        };
        await jobRepository.AddAsync(job, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await jobRepository.GetJobAsync(viewer, jobId, today, cancellationToken);
        return Result.Ok(new AddTasksResponse(JobResponse.From(viewer, created!), plan.Skipped, plan.Warnings));
    }

    // D4 "give work": someone in a lower role inside the user's scope (an Admin may give work to anyone in a tracking role).
    // A Branch Manager and an Assistant Branch Manager are the same level, so they never give each other work.
    private static bool CanGiveWork(TrackingViewer viewer, TrackingOfficer assignee)
    {
        if (assignee.EmployeeId == viewer.EmployeeId || !TrackingRole.IsTrackingPost(assignee.PostName)) return false;
        if (viewer.Role == TrackingRole.Admin) return true;
        return TrackingRole.Rank(assignee.PostName) < TrackingRole.Rank(viewer.Role)
            && viewer.UnitIdsInScope.Contains(assignee.UnitId);
    }
}
#endregion
