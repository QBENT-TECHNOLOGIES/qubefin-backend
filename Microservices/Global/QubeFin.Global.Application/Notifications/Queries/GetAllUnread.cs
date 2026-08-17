using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Notifications.Queries;

#region --- QUERY ---
public record GetAllQuery(Guid EmployeeId) : IRequest<Result<IEnumerable<Notification>>>;
#endregion

#region --- VALIDATOR ---
public class GetAllQueryValidator : AbstractValidator<GetAllQuery>
{
    public GetAllQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetAllQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetAllQuery, Result<IEnumerable<Notification>>>
{
    public async Task<Result<IEnumerable<Notification>>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetAllAsync(request.EmployeeId);
        return Result.Ok(notifications);
    }
}
#endregion
