using FluentResults;
using FluentValidation;
using MediatR;
using OtpNet;
using QubeFin.Auth.Application.Accounts.Model;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---

public record VerifyForgotPasswordMfaCommand(ForgotPasswordVerifyMfaRequest Request) : IRequest<Result<VerifyForgotPasswordMfaResponse>>;

#endregion

#region --- RESPONSE ---

public record VerifyForgotPasswordMfaResponse(string ResetToken);

#endregion

#region --- VALIDATOR ---

public class VerifyForgotPasswordMfaCommandValidator
    : AbstractValidator<VerifyForgotPasswordMfaCommand>
{
    public VerifyForgotPasswordMfaCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull();

        RuleFor(x => x.Request.SessionToken)
            .NotEmpty()
            .WithMessage("Session token is required.");

        RuleFor(x => x.Request.MfaCode)
            .NotEmpty()
            .Length(6)
            .WithMessage("MFA code must be 6 digits.");
    }
}

#endregion

#region --- HANDLER ---

internal sealed class VerifyForgotPasswordMfaCommandHandler(IAuthRepository authRepository, IUnitOfWork unitOfWork) : IRequestHandler<VerifyForgotPasswordMfaCommand, Result<VerifyForgotPasswordMfaResponse>>
{
    public async Task<Result<VerifyForgotPasswordMfaResponse>> Handle(VerifyForgotPasswordMfaCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (!req.SessionToken.StartsWith("FP_", StringComparison.Ordinal))
        {
            return new ValidationError("Invalid password reset session.");
        }

        var session = await authRepository.GetUserSessionByTokenAsync(req.SessionToken);

        if (session is null)
        {
            return new ValidationError("Invalid or expired password reset session.");
        }

        if (session.LoginTime.AddMinutes(10) < DateTime.UtcNow)
        {
            return new ValidationError("Password reset session has expired.");
        }

        if (session.LogoutTime != null)
        {
            return new ValidationError("Password reset session is no longer active.");
        }

        var user = await authRepository.GetUserBySessionTokenAsync(req.SessionToken);

        if (user is null)
        {
            return new RecordNotFoundError("User not found.");
        }

        if (string.IsNullOrWhiteSpace(user.MfaSecret))
        {
            return new ValidationError("MFA is not configured for this account.");
        }

        var secretBytes = Base32Encoding.ToBytes(user.MfaSecret);
        var totp = new Totp(secretBytes);
        var mfaVerified = totp.VerifyTotp(req.MfaCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

        if (!mfaVerified)
        {
            return new ValidationError("Invalid MFA code.");
        }

        session.MarkMfaVerified();

        authRepository.UpdateSession(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new VerifyForgotPasswordMfaResponse(session.SessionToken));
    }
}

#endregion