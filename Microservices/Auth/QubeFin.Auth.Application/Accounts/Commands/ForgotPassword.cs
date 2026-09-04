using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Auth.Application.Accounts.Model;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---
public record ForgotPasswordCommand(ForgotPasswordInitiateRequest user) : IRequest<Result<ForgotPasswordResponse>>;
#endregion

#region --- RESPONSE ---
public record ForgotPasswordResponse(string SessionToken);
#endregion

#region --- VALIDATOR ---

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.user).NotNull();
        RuleFor(x => x.user.UserName).NotEmpty().WithMessage("Username is required.");
    }
}

#endregion

#region --- HANDLER ---
internal sealed class ForgotPasswordCommandHandler(IAuthRepository authRepository, IUnitOfWork unitOfWork) : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetUserByUserNameAsync(request.user.UserName);
        if (user is null)
        {
            throw new Exception("User not found.");
        }

        if (string.IsNullOrWhiteSpace(user.MfaSecret))
        {
            return new ValidationError("MFA is not configured for this account.");
        }

        var sessionToken = "FP_" + Guid.NewGuid().ToString("N");

        var userSession = UserSession.Create(Guid.NewGuid(), user.Id, sessionToken, request.user.DeviceId, request.user.UserAgent);
        await authRepository.CreateUserSessionAsync(userSession);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new ForgotPasswordResponse(sessionToken));
    }
}
#endregion