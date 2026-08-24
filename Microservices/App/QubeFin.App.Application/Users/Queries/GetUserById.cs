using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Persistence.Repositories;
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
internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository, QubeFinDataContext context)
    : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetExistingUser(request.Id, cancellationToken);
        return new GetUserByIdResponse(
            existingUser.Id, existingUser.UserName, 
            existingUser.EmployeeId, existingUser.EmployeeName == null ? string.Empty : existingUser.EmployeeName, existingUser.MfaSecret, existingUser.HasMfaEnabled, existingUser.IsActive, existingUser.IsSuperAdmin);
        
    }
}
#endregion
