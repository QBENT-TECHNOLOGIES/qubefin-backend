using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- QUERY --
public record CancelRequestCommand(Guid Id, string? reason, Guid UserId) : IRequest<Result<CancelRequestResponse>>;
#endregion

#region --- RESPONSE ---
public record CancelRequestResponse(bool success, string Message);
#endregion

#region --- HANDLER ---
internal sealed class CancelRequestCommandHandler(ILeaveRepository leaveRepository, IUnitOfWork unitOfWork) : IRequestHandler<CancelRequestCommand, Result<CancelRequestResponse>>
{
    public async Task<Result<CancelRequestResponse>> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await leaveRepository.CancelAsync(request.Id, request.reason, request.UserId);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CancelRequestResponse(response, $"{(response == true ? "Leave Request cancelled successfully" : "Failed to cancel Leave Request")}"));
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while cancelling the leave request: {ex.Message}", ex);
        }
    }
}
#endregion