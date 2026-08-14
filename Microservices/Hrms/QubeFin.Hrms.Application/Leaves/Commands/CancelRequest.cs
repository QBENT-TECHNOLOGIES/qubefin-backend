using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- QUERY --
public record CancelRequestCommand(Guid Id, string? reason, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- HANDLER ---
internal sealed class CancelRequestCommandHandler(ILeaveRepository leaveRepository, IUnitOfWork unitOfWork) : IRequestHandler<CancelRequestCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await leaveRepository.CancelAsync(request.Id, request.reason, request.UserId);
            if (!response)
            {
                return Result.Fail("Failed to cancel Leave Request");
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Leave Request cancelled successfully");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
#endregion