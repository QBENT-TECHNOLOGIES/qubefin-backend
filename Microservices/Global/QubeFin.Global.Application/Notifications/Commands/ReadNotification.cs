using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Notifications.Commands;

#region --- COMMAND ---
public record ReadNotificationCommand(Guid Id) : IRequest<Result<ReadNotificationResponse>>;
#endregion

#region --- RESPONSE ---
public record ReadNotificationResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class ReadNotificationCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : IRequestHandler<ReadNotificationCommand, Result<ReadNotificationResponse>>
{
    public async Task<Result<ReadNotificationResponse>> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
    {
        var success = await notificationRepository.MarkAsReadAsync(request.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new ReadNotificationResponse(true));
    }
}
#endregion
