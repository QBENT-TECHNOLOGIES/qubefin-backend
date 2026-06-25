using FluentResults;
using MediatR;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Core.Security;
using QubeFin.Persistence;

namespace QubeFin.Auh.Application.Accounts.Commands;

#region --- COMMAND ---
public record ValidtateLoginCommand(string UserName, string Password) : IRequest<Result<ValidtateLoginResponse>>;
#endregion

#region --- RESPONSE ---
public record ValidtateLoginResponse(string SessionToken);
#endregion

#region --- HANDLER ---
internal class ValidtateLoginCommandHandler(IAuthRepository authRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ValidtateLoginCommand, Result<ValidtateLoginResponse>>
{
    public async Task<Result<ValidtateLoginResponse>> Handle(ValidtateLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await authRepository.ValidateLoginAsync(request.UserName, request.Password);
        if (user is null)
        {
            return new RecordNotFoundError($"User with email {request.UserName} not found");
        }

        if (!user.IsActive)
        {
            return new ValidationError("User is inactive. Please ask administrator to activate the user.");
        }

        var sessionToken = SecureTokenGenerator.Generate(32);

        //var userSession = UserSession.Create(new UserSessionId(Guid.NewGuid()), user.Id, sessionToken);
        //await authRepository.CreateUserSessionAsync(userSession);
        //await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new ValidtateLoginResponse(sessionToken));
    }
}
#endregion