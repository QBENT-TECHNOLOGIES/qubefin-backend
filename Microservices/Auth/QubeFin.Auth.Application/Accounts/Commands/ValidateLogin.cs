using FluentResults;
using MediatR;
using QubeFin.Auth.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Core.Security;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---
public record ValidateLoginCommand(string UserName, string Password, string? DeviceId, string? UserAgent) : IRequest<Result<ValidtateLoginResponse>>;
#endregion

#region --- RESPONSE ---
public record ValidtateLoginResponse(string SessionToken);
#endregion

#region --- HANDLER ---
internal class ValidtateLoginCommandHandler(IAuthRepository authRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ValidateLoginCommand, Result<ValidtateLoginResponse>>
{
    public async Task<Result<ValidtateLoginResponse>> Handle(ValidateLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await authRepository.ValidateLoginAsync(request.UserName, request.Password);
            if (user is null)
            {
                return new RecordNotFoundError($"User with username {request.UserName} not found");
            }

            if (!user.IsActive)
            {
                return new ValidationError("User is inactive. Please ask administrator to activate the user.");
            }

            var sessionToken = SecureTokenGenerator.Generate(32);

            var userSession = UserSession.Create(Guid.NewGuid(), user.Id, sessionToken, request.DeviceId, request.UserAgent);
            await authRepository.CreateUserSessionAsync(userSession);

            if (!string.IsNullOrWhiteSpace(request.DeviceId))
            {
                // 1. This device is bound to another user and not yet released
                string? assignedUserName = await authRepository.DeviceTagUsername(user.Id, request.DeviceId);
                if (assignedUserName is not null)
                {
                    return new ValidationError($"Device is allocated to {assignedUserName}, please contact admin !");
                }

                // 2. null = user has no unreleased device, true = this device, false = another device
                var deviceStatus = await authRepository.ValidateDevice(user.Id, request.DeviceId);
                if (deviceStatus == false)
                {
                    // User is bound to a different device that is not yet released
                    return new ValidationError("Your account is bind to another device, please contact admin !");
                }

                // 3. Both user and device are free: bind them
                if (deviceStatus is null)
                {
                    var newDevice = UserDevice.Create(Guid.NewGuid(), user.Id, request.DeviceId);
                    authRepository.RegisterDevice(newDevice);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new ValidtateLoginResponse(sessionToken));
        }
        catch (Exception ex)
        {
            return Result.Fail(new ValidationError($"{ex.Message}"));
        }
    }
}
#endregion