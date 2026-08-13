using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Notifications.Queries;

#region --- QUERY ---
public record GetAllUnreadQuery(Guid EmployeeId) : IRequest<Result<GetAllUnreadResponse>>;
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
#region --- RESPONSE ---
public record GetAllUnreadResponse(IEnumerable<Notification> Notifications);
#endregion

#region --- HANDLER ---
internal sealed class GetAllUnreadQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetAllUnreadQuery, Result<GetAllUnreadResponse>>
{
    public async Task<Result<GetAllUnreadResponse>> Handle(GetAllUnreadQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetAllUnreadAsync(request.EmployeeId);
        return new GetAllUnreadResponse(notifications);
    }
}
#endregion
