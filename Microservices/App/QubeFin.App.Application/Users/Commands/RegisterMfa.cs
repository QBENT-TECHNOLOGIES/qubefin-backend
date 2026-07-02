using FluentResults;
using MediatR;
using OtpNet;
using QRCoder;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record RegisterMfaCommand(string SessionToken) : IRequest<Result<RegisterMfaResponse>>;
#endregion

#region --- RESPONSE ---
public record RegisterMfaResponse(string MfaKey, string QrCodeImageUrl, string SessionToken);
#endregion

#region --- HANDLER ---
internal sealed class RegisterMfaCommandHandler(IUserRepository userRepository)
    : IRequestHandler<RegisterMfaCommand, Result<RegisterMfaResponse>>
{
    public async Task<Result<RegisterMfaResponse>> Handle(RegisterMfaCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserBySessionTokenAsync(request.SessionToken);
        if (user is null)
        {
            return new RecordNotFoundError($"No User found for the session {request.SessionToken}");
        }

        if (user.HasMfaEnabled)
        {
            return new ValidationError("MFA is already enabled for this user.");
        }

        string issuer = "QbentIntranet";
        string totpUri = new OtpUri(OtpType.Totp, user.MfaSecret, user.UserName, issuer).ToString();

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(totpUri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new Base64QRCode(qrCodeData);
        var base64Image = qrCode.GetGraphic(20);

        return Result.Ok(new RegisterMfaResponse(user.MfaSecret, $"data:image/png;base64,{base64Image}", request.SessionToken));
    }
}
#endregion