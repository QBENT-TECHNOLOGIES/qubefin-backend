using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Queries;

#region --- QUERY ---
// OrganisationUnitId: only jobs whose assignee can take that branch's work. AssigneeId: only jobs held by this employee.
// SearchText: job code, title or assignee name.
public record GetOpenJobsQuery(Guid UserId, Guid? OrganisationUnitId, Guid? AssigneeId, string? SearchText) : IRequest<Result<List<JobSummaryResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetOpenJobsQueryValidator : AbstractValidator<GetOpenJobsQuery>
{
    public GetOpenJobsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.SearchText).MaximumLength(200).WithMessage("Search text can be at most 200 characters.");
    }
}
#endregion

#region --- HANDLER ---
// Open jobs (Assigned, InProgress, Delayed) in the user's branches (Global.Fn_GetUserWiseBranches), soonest complete-by first
internal sealed class GetOpenJobsQueryHandler(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository)
    : IRequestHandler<GetOpenJobsQuery, Result<List<JobSummaryResponse>>>
{
    public async Task<Result<List<JobSummaryResponse>>> Handle(GetOpenJobsQuery query, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(query.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var rows = await jobRepository.GetOpenJobsAsync(
            new OpenJobFilter(viewer.UserId, query.OrganisationUnitId, query.AssigneeId, query.SearchText),
            TrackingClock.Today(),
            cancellationToken);

        return Result.Ok(rows.Select(JobSummaryResponse.From).ToList());
    }
}
#endregion
