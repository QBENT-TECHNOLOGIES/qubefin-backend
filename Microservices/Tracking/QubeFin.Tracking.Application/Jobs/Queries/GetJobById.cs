using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Queries;

#region --- QUERY ---
public record GetJobByIdQuery(Guid UserId, Guid JobId) : IRequest<Result<JobResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetJobByIdQueryValidator : AbstractValidator<GetJobByIdQuery>
{
    public GetJobByIdQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetJobByIdQueryHandler(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository) : IRequestHandler<GetJobByIdQuery, Result<JobResponse>>
{
    public async Task<Result<JobResponse>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var job = await jobRepository.GetJobAsync(viewer, request.JobId, TrackingClock.Today(), cancellationToken);
        if (job is null) return new RecordNotFoundError("Job not found");

        var summary = JobSummaryResponse.From(job.Summary);
        return Result.Ok(new JobResponse(
            summary.Id,
            summary.JobCode,
            summary.JobTitle,
            summary.Status,
            summary.AssigneeId,
            summary.AssigneeName,
            summary.AssigneeRole,
            summary.UnitName,
            summary.OrganisationUnitIds,
            summary.BranchNames,
            summary.CreatedByName,
            summary.CreatedOn,
            summary.StartDate,
            summary.EndDate,
            summary.TaskCount,
            summary.DoneCount,
            summary.OpenCount,
            summary.SelfAddedCount,
            summary.Types,
            summary.TransferJobRefId,
            summary.TransferJobRefCode,
            job.JobDescription,
            job.CreatedBy,
            job.Tasks.Select(JobTaskResponse.From).ToList(),
            JobPermissionRules.For(viewer, job)));
    }
}
#endregion
