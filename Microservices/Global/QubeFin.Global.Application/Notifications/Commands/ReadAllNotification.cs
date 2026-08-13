using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Notifications.Commands;

#region --- COMMAND ---
public record ReadAllNotificationCommand(Guid EmployeeId) : IRequest<Result<ReadAllNotificationResponse>>;
#endregion

#region --- RESPONSE ---
public record ReadAllNotificationResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class ReadAllNotificationCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : IRequestHandler<ReadAllNotificationCommand, Result<ReadAllNotificationResponse>>
{
    public async Task<Result<ReadAllNotificationResponse>> Handle(ReadAllNotificationCommand request, CancellationToken cancellationToken)
    {
        var success = await notificationRepository.MarkAllReadAsync(request.EmployeeId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new ReadAllNotificationResponse(true));
    }
}
#endregion
