using FluentResults;
using MediatR;
using OtpNet;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Core.Security;
using QubeFin.Persistence;
using System.Security.Claims;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---
public record VerifyMfaCommand(string MfaCode, string SessionToken) : IRequest<Result<VerifyMfaResponse>>;
#endregion

#region --- RESPONSE ---
public record VerifyMfaResponse(string AccessToken, string RefreshToken);
#endregion

#region --- HANDLER ---
internal class VerifyMfaCommandHandler(IAuthRepository authRepository, IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator)
    : IRequestHandler<VerifyMfaCommand, Result<VerifyMfaResponse>>
{
    public async Task<Result<VerifyMfaResponse>> Handle(VerifyMfaCommand request, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetUserBySessionTokenAsync(request.SessionToken);
        if (user is null)
        {
            return new RecordNotFoundError($"No User found for the session {request.SessionToken}");
        }

        var secretBytes = Base32Encoding.ToBytes(user.MfaSecret);
        var totp = new Totp(secretBytes);
        var mfaVerified = totp.VerifyTotp(request.MfaCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        if (!mfaVerified)
        {
            return new RecordNotFoundError($"MFA Code not matched");
        }

        var permissions = await authRepository.GetPermissionsAsync(user.Id);

        var claims = permissions.Select(p => new Claim("permission", p)).ToList();
        claims.Add(new Claim("UserId", user.Id.ToString()));
        if (user.EmployeeId != null) {
            claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
            //claims.Add(new Claim("EmployeeName", user.Employee.FullName.ToString()));
            //claims.Add(new Claim("Role", user.Role.Name.ToString()));
        }
        
        var accessToken = tokenGenerator.GenerateAccessToken(claims);
        var refreshToken = tokenGenerator.GenerateRefreshToken();

        var userSession = await authRepository.GetUserSessionByTokenAsync(request.SessionToken);
        if (userSession is null)
        {
            return new RecordNotFoundError($"No User Session found for {request.SessionToken}");
        }

        userSession.Open(accessToken, refreshToken);
        authRepository.UpdateSession(userSession);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new VerifyMfaResponse(accessToken, refreshToken));
    }
}
#endregion