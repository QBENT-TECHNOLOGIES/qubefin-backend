using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- QUERY --
public record SubmitRequestCommand(Guid Id, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- HANDLER ---
internal sealed class SubmitRequestCommandHandler(ILeaveRepository leaveRepository, IUnitOfWork unitOfWork) : IRequestHandler<SubmitRequestCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SubmitRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await leaveRepository.SubmitAsync(request.Id, request.UserId);
            if (!response)
            {
                return Result.Fail("Failed to submit Leave Request");
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Leave Request submitted successfully");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
#endregion