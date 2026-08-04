using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- QUERY --
public record SubmitRequestCommand(Guid Id, Guid UserId) : IRequest<Result<SubmitRequestResponse>>;
#endregion

#region --- RESPONSE ---
public record SubmitRequestResponse(bool success, string Message);
#endregion

#region --- HANDLER ---
internal sealed class SubmitRequestCommandHandler(ILeaveRepository leaveRepository, IUnitOfWork unitOfWork) : IRequestHandler<SubmitRequestCommand, Result<SubmitRequestResponse>>
{
    public async Task<Result<SubmitRequestResponse>> Handle(SubmitRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await leaveRepository.SubmitAsync(request.Id, request.UserId);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new SubmitRequestResponse(response, $"{(response == true ? "Leave Request submitted successfully" : "Failed to submit Leave Request")}"));
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while submitting the leave request: {ex.Message}", ex);
        }
    }
}
#endregion