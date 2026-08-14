using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Notifications.Queries;

#region --- QUERY ---
public record GetAllUnreadQuery(Guid EmployeeId) : IRequest<Result<IEnumerable<Notification>>>;
#endregion

#region --- VALIDATOR ---
public class GetAllUnreadQueryValidator : AbstractValidator<GetAllUnreadQuery>
{
    public GetAllUnreadQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetAllUnreadQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetAllUnreadQuery, Result<IEnumerable<Notification>>>
{
    public async Task<Result<IEnumerable<Notification>>> Handle(GetAllUnreadQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetAllUnreadAsync(request.EmployeeId);
        return Result.Ok(notifications);
    }
}
#endregion
