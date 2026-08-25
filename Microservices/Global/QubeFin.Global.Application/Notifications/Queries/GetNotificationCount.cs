using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.Notifications.Queries;
#region --- QUERY ---
public record GetNotificationCountQuery(Guid EmployeeId) : IRequest<Result<int>>;
#endregion

#region --- VALIDATOR ---
public class GetNotificationCountQueryValidator : AbstractValidator<GetNotificationCountQuery>
{
    public GetNotificationCountQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetNotificationCountQueryHandler(INotificationRepository notificationRepository) : IRequestHandler<GetNotificationCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var count = await notificationRepository.GetCountAsync(request.EmployeeId);
        return Result.Ok(count);
    }
}
#endregion

