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
public record ExtendJobRequest(DateOnly? EndDate, string? Remarks);

public record ExtendJobCommand(Guid UserId, Guid JobId, ExtendJobRequest Request) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class ExtendJobCommandValidator : AbstractValidator<ExtendJobCommand>
{
    public ExtendJobCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
        RuleFor(v => v.Request.EndDate).NotNull().WithMessage("Select the new complete-by date.");
        RuleFor(v => v.Request.EndDate)
            .Must(d => d >= TrackingClock.Today()).WithMessage("The new date cannot be in the past.")
            .When(v => v.Request.EndDate.HasValue);
        RuleFor(v => v.Request.Remarks).NotEmpty().WithMessage("A reason is required.");
        RuleFor(v => v.Request.Remarks).MaximumLength(1000).WithMessage("The reason can be at most 1000 characters.");
    }
}
#endregion

#region --- HANDLER ---
// Moves the complete-by date later. Tasks do not change; a Delayed job whose new date is in the future is back to Assigned or InProgress on read.
internal sealed class ExtendJobCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ExtendJobCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ExtendJobCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var today = TrackingClock.Today();
        var detail = await jobRepository.GetJobAsync(viewer, command.JobId, today, cancellationToken);
        if (detail is null) return new RecordNotFoundError("Job not found");
        if (!JobPermissionRules.For(viewer, detail).CanUpdate)
        {
            return new ForbiddenError("Only the person who created this job, someone above them, or a manager above the assignee can extend it.");
        }

        var status = Enum.Parse<JobStatus>(detail.Summary.Status);
        if (status is not (JobStatus.Assigned or JobStatus.InProgress or JobStatus.Delayed))
        {
            return new ValidationError($"{detail.Summary.JobCode} is {status} and cannot be extended.");
        }

        var newEndDate = command.Request.EndDate!.Value;
        if (detail.Summary.EndDate.HasValue && newEndDate <= detail.Summary.EndDate.Value)
        {
            return new ValidationError("The new date must be later than the current one.");
        }

        var job = (await jobRepository.GetJobForUpdateAsync(command.JobId, cancellationToken))!;
        var performedAt = DateTimeOffset.Now;
        var oldEndDate = job.EndDate;
        job.EndDate = newEndDate;
        job.ModifiedBy = viewer.UserId;
        job.ModifiedOn = performedAt.DateTime;
        await jobRepository.AddHistoryAsync(
            JobHistoryEntries.New(job.Id, JobHistoryAction.DateExtended, viewer.UserId, performedAt,
                toAssigneeId: job.AssignTo, oldCompleteBy: oldEndDate, newCompleteBy: newEndDate, remarks: command.Request.Remarks!.Trim()),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{job.JobCode} complete-by date extended from {oldEndDate:dd MMM yyyy} to {newEndDate:dd MMM yyyy}.");
    }
}
#endregion
