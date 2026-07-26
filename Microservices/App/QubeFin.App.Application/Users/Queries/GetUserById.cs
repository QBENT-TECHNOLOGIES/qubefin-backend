using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUserByIdQuery(Guid Id) : IRequest<Result<GetUserByIdResponse>>;
#endregion

#region --- RESPONSE ---
public record GetUserByIdResponse(Guid Id, string UserName, Guid? EmployeeId, string Employee, string MfaSecret, bool HasMfaEnabled, bool IsActive, bool IsSuperAdmin);
#endregion

#region --- HANDLER ---
internal sealed class GetUserByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context
            .TblUsers
            .Include(m => m.Employee)
            .Where(m => m.Id == request.Id)
            .Select(m => new GetUserByIdResponse(m.Id, m.UserName, m.EmployeeId, m.Employee == null ? string.Empty : m.Employee.FullName, m.MfaSecret, m.HasMfaEnabled, m.IsActive, m.IsSuperAdmin))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return new RecordNotFoundError($"User not found for the given Id");
        }
        return Result.Ok(user);
    }
}
#endregion
