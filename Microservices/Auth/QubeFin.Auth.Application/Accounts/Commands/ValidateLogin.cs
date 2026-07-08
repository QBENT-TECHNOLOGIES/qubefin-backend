using FluentResults;
using MediatR;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Core.Security;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auh.Application.Accounts.Commands;

#region --- COMMAND ---
public record ValidtateLoginCommand(string UserName, string Password, string? DeviceId, string? UserAgent) : IRequest<Result<ValidtateLoginResponse>>;
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

        var userSession = UserSession.Create(Guid.NewGuid(), user.Id, sessionToken, request.DeviceId, request.UserAgent);
        await authRepository.CreateUserSessionAsync(userSession);
       
        if (request.DeviceId is not null)
        {
            var deviceStatus = await authRepository.ValidateDevice(user.Id, request.DeviceId);
            if (deviceStatus is null) 
            {
                var newDevice = UserDevice.Create(Guid.NewGuid(), user.Id, request.DeviceId);
                authRepository.RegisterDevice(newDevice);        
            }
            else if (!deviceStatus.Value)
            {
                return new ValidationError("Device is allocated to another user, please contact admin !");
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new ValidtateLoginResponse(sessionToken));
    }
}
#endregion