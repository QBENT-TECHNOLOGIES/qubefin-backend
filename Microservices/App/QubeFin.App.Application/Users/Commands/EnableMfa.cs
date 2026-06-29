using FluentResults;
using MediatR;
using OtpNet;
using QRCoder;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record EnableMfaCommand(string SessionToken) : IRequest<Result<EnableMfaResponse>>;
#endregion

#region --- RESPONSE ---
public record EnableMfaResponse(string SessionToken);
#endregion

#region --- HANDLER ---
internal sealed class EnableMfaCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<EnableMfaCommand, Result<EnableMfaResponse>>
{
    public async Task<Result<EnableMfaResponse>> Handle(EnableMfaCommand request, CancellationToken cancellationToken)
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

        user.EnableMfa();
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new EnableMfaResponse(request.SessionToken));
    }
}
#endregion