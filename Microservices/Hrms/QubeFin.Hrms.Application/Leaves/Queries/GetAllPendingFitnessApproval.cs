using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Leaves.Queries;

public class GetAllPendingFitnessApproval
{
}


#region --- QUERY ---
public record GetAllPendingFitnessApprovalQuery() : IRequest<Result<List<GetAllPendingFitnessApprovalResposne>?>>;
#endregion

#region --- HANDLER ---
internal sealed class GetAllPendingFitnessApprovalQueryHandler(ILeaveRepository LeaveRepository, IFileStorageRepository fileStorageRepository) :
    IRequestHandler<GetAllPendingFitnessApprovalQuery, Result<List<GetAllPendingFitnessApprovalResposne>?>>
{
    public async Task<Result<List<GetAllPendingFitnessApprovalResposne>?>> Handle(GetAllPendingFitnessApprovalQuery request, CancellationToken cancellationToken)
    {
        var result = await LeaveRepository.GetPendingFitnessApprovalList(cancellationToken);
        return Result.Ok((List<GetAllPendingFitnessApprovalResposne>?)result);
    }
}
#endregion
