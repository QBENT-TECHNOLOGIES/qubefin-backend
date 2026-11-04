using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Lookups.Queries;

#region --- QUERY ---
public record GetTrackingViewerQuery(Guid UserId) : IRequest<Result<TrackingViewerResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetTrackingViewerQueryValidator : AbstractValidator<GetTrackingViewerQuery>
{
    public GetTrackingViewerQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record TrackingViewerResponse(
    Guid? EmployeeId,
    string Name,
    string Role,
    string UnitName,
    IReadOnlyList<Guid> OrganisationUnitIds,
    bool CanCreateJobs);
#endregion

#region --- HANDLER ---
internal sealed class GetTrackingViewerQueryHandler(ITrackingScopeRepository scopeRepository) : IRequestHandler<GetTrackingViewerQuery, Result<TrackingViewerResponse>>
{
    public async Task<Result<TrackingViewerResponse>> Handle(GetTrackingViewerQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var canCreateJobs = viewer.Role != TrackingRole.CreditOfficer
            && await scopeRepository.HasAssignableEmployeesAsync(viewer, cancellationToken);

        return Result.Ok(new TrackingViewerResponse(
            viewer.EmployeeId,
            viewer.Name,
            viewer.Role,
            viewer.UnitName,
            viewer.BranchIds,
            canCreateJobs));
    }
}
#endregion
