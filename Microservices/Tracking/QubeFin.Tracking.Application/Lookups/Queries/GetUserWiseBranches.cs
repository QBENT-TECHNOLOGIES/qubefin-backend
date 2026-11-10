using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Lookups.Queries;

#region --- QUERY ---
public record GetUserWiseBranchesQuery(Guid UserId) : IRequest<Result<List<UserWiseBranchItem>>>;
#endregion

#region --- VALIDATOR ---
public class GetUserWiseBranchesQueryValidator : AbstractValidator<GetUserWiseBranchesQuery>
{
    public GetUserWiseBranchesQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
    }
}
#endregion

#region --- HANDLER ---
// Branch dropdown (Global.Fn_GetUserWiseBranches): BM one branch, AM the area's, DM the division's, super admin all
internal sealed class GetUserWiseBranchesQueryHandler(ITrackingScopeRepository scopeRepository) : IRequestHandler<GetUserWiseBranchesQuery, Result<List<UserWiseBranchItem>>>
{
    public async Task<Result<List<UserWiseBranchItem>>> Handle(GetUserWiseBranchesQuery request, CancellationToken cancellationToken) =>
        Result.Ok(await scopeRepository.GetUserWiseBranchesAsync(request.UserId, cancellationToken));
}
#endregion
