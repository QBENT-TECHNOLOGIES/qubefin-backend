using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Lookups.Queries;

#region --- QUERY ---
public record GetTrackingBranchesQuery(Guid UserId) : IRequest<Result<List<TrackingBranch>>>;
#endregion

#region --- VALIDATOR ---
public class GetTrackingBranchesQueryValidator : AbstractValidator<GetTrackingBranchesQuery>
{
    public GetTrackingBranchesQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
    }
}
#endregion

#region --- HANDLER ---
// The user's branches come from Global.Fn_GetUserWiseBranches, the same scope as the group and member lists:
// super admin every branch, DM / AM / BM the branches under their unit, everyone else their own branch
internal sealed class GetTrackingBranchesQueryHandler(ITrackingScopeRepository scopeRepository) : IRequestHandler<GetTrackingBranchesQuery, Result<List<TrackingBranch>>>
{
    public async Task<Result<List<TrackingBranch>>> Handle(GetTrackingBranchesQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var branches = await scopeRepository.GetUserWiseBranchesAsync(request.UserId, cancellationToken);
        return Result.Ok(branches);
    }
}
#endregion
