using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Auth.Application.Accounts.Commands;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Core.Security;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;
using System.Security.Claims;

namespace QubeFin.Auth.Application.Accounts.Queries;

#region --- QUERY ---
public record ValidateRefreshTokenQuery(string RefreshToken) : IRequest<Result<ValidateRefreshTokenResponse>>;
#endregion

#region --- RESPONSE ---
public record ValidateRefreshTokenResponse(string AccessToken, string RefreshToken);
#endregion

#region --- HANDLER ---
internal class ValidateRefreshTokenQueryHandler(QubeFinDataContext context, IAuthRepository authRepository, IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator)
    : IRequestHandler<ValidateRefreshTokenQuery, Result<ValidateRefreshTokenResponse>>
{
    public async Task<Result<ValidateRefreshTokenResponse>> Handle(ValidateRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var userSessionEntity = await context.TblUserSessions.FirstOrDefaultAsync(m => m.RefreshToken == request.RefreshToken);
        if (userSessionEntity is null)
        {
            return new UnauthorizedError($"User Session not found");
        }

        var userSession = userSessionEntity.ToDomain();
        var user = await authRepository.GetUserBySessionTokenAsync(userSession.SessionToken);
        if (user is null)
        {
            return new RecordNotFoundError($"No User found for the session");
        }

        if (!user.IsActive)
        {
            return new ForbiddenError("User not active or does not have any permission");
        }

        var permissions = await authRepository.GetPermissionsAsync(userSession.UserId);

        var claims = permissions.Select(p => new Claim("permission", p)).ToList();
        claims.Add(new Claim("UserId", userSession.UserId.ToString()));
        if (user.EmployeeId != null)
        {
            claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
            //claims.Add(new Claim("EmployeeName", user.Employee.FullName.ToString()));
            //claims.Add(new Claim("Role", user.Role.Name.ToString()));
        }

        var accessToken = tokenGenerator.GenerateAccessToken(claims);
        var refreshToken = tokenGenerator.GenerateRefreshToken();

        userSession.Open(accessToken, refreshToken);
        authRepository.UpdateSession(userSession);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new ValidateRefreshTokenResponse(accessToken, refreshToken));
    }
}
#endregion
