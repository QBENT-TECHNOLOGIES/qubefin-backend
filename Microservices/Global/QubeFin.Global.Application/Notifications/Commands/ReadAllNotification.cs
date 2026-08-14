using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Notifications.Commands;

#region --- COMMAND ---
public record ReadAllNotificationCommand(Guid EmployeeId) : IRequest<Result<bool>>;
#endregion

#region --- HANDLER ---
internal sealed class ReadAllNotificationCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : 
    IRequestHandler<ReadAllNotificationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ReadAllNotificationCommand request, CancellationToken cancellationToken)
    {
        var success = await notificationRepository.MarkAllReadAsync(request.EmployeeId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(success);
    }
}
#endregion
