using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Auth.Application.Accounts.Model;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---

public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result<string>>;

#endregion

#region --- VALIDATOR ---

public class ResetPasswordCommandValidator
    : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.SessionToken).NotEmpty().WithMessage("Session token is required.");
        RuleFor(x => x.Request.NewPassword).NotEmpty().WithMessage("New password is required.");
    }
}

#endregion

#region --- HANDLER ---

internal sealed class ResetPasswordCommandHandler(QubeFinDataContext context, IAuthRepository authRepository, IUnitOfWork unitOfWork) : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
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

        if (session.LogoutTime != null)
        {
            return new ValidationError("Password reset session is no longer active.");
        }

        if (session.LoginTime.AddMinutes(10) < DateTime.UtcNow)
        {
            return new ValidationError("Password reset session has expired.");
        }

        if (!session.IsMfaVerified)
        {
            return new ValidationError("MFA verification is required.");
        }

        var user = await context.TblUsers.FirstOrDefaultAsync(x => x.Id == session.UserId, cancellationToken);

        if (user is null)
        {
            return new RecordNotFoundError("User not found.");
        }

        var passwordHasher = new PasswordHasher<AppUser>();
        var appUser = new AppUser(user.UserName, req.NewPassword);
        var hashedPassword = passwordHasher.HashPassword(appUser, req.NewPassword);

        user.Password = hashedPassword;
        session.Close();

        authRepository.UpdateSession(session);
        context.TblUsers.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Password reset successfully.");
    }
}

#endregion