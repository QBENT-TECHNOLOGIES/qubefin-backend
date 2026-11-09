using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Lookups.Queries;

#region --- QUERY ---
public record GetFieldOfficersQuery(Guid UserId, Guid? OrganizationUnitId) : IRequest<Result<List<TrackingFieldOfficer>>>;
#endregion

#region --- VALIDATOR ---
public class GetFieldOfficersQueryValidator : AbstractValidator<GetFieldOfficersQuery>
{
    public GetFieldOfficersQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetFieldOfficersQueryHandler(ITrackingScopeRepository scopeRepository) : IRequestHandler<GetFieldOfficersQuery, Result<List<TrackingFieldOfficer>>>
{
    public async Task<Result<List<TrackingFieldOfficer>>> Handle(GetFieldOfficersQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        // Global.USP_GetFieldOfficers, scoped by Global.Fn_GetUserWiseBranches like the branch list and the group / member lists
        var officers = await scopeRepository.GetFieldOfficersAsync(request.UserId, request.OrganizationUnitId, cancellationToken);
        return Result.Ok(officers);
    }
}
#endregion
