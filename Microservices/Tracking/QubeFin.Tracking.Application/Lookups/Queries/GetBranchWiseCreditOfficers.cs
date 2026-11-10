using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Lookups.Queries;

#region --- QUERY ---
public record GetBranchWiseCreditOfficersQuery(Guid UserId, Guid? OrganisationUnitId) : IRequest<Result<List<BranchCreditOfficerItem>>>;
#endregion

#region --- VALIDATOR ---
public class GetBranchWiseCreditOfficersQueryValidator : AbstractValidator<GetBranchWiseCreditOfficersQuery>
{
    public GetBranchWiseCreditOfficersQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.OrganisationUnitId).NotEmpty().WithMessage("Select a branch.");
    }
}
#endregion

#region --- HANDLER ---
// Credit Officer dropdown for a branch picked from the user-wise branch list (Global.USP_GetBranchWiseCreditOfficers).
// A branch that is not one of the user's branches is not found; a Credit Officer gets only themself.
internal sealed class GetBranchWiseCreditOfficersQueryHandler(ITrackingScopeRepository scopeRepository) : IRequestHandler<GetBranchWiseCreditOfficersQuery, Result<List<BranchCreditOfficerItem>>>
{
    public async Task<Result<List<BranchCreditOfficerItem>>> Handle(GetBranchWiseCreditOfficersQuery request, CancellationToken cancellationToken)
    {
        var branchId = request.OrganisationUnitId!.Value;
        var branches = await scopeRepository.GetUserWiseBranchesAsync(request.UserId, cancellationToken);
        if (branches.All(b => b.Id != branchId)) return new RecordNotFoundError("Branch not found");

        return Result.Ok(await scopeRepository.GetBranchWiseCreditOfficersAsync(request.UserId, branchId, cancellationToken));
    }
}
#endregion
