using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Notifications.Commands;

#region --- COMMAND ---
public record ReadNotificationCommand(Guid Id) : IRequest<Result<bool>>;
#endregion

#region --- HANDLER ---
internal sealed class ReadNotificationCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : 
    IRequestHandler<ReadNotificationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
    {
        var success = await notificationRepository.MarkAsReadAsync(request.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(success);
    }
}
#endregion
