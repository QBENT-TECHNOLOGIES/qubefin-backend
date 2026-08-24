using FluentResults;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record UpdateUserCommand(Guid id, string UserName, bool isActive, bool hasMfaEnabled, Guid? UserId) : IRequest<Result<string>>;
#endregion

#region --- HANDLER ---
internal sealed class UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUserName = await userRepository.GetExsitingUserByUserName(request.id, request.UserName);
        if (existingUserName)
        {
            return new ValidationError("Employee already exist with same code.");
        }

        var existingUser = await userRepository.GetExistingUser(request.id, cancellationToken);

        existingUser.Update(request.UserName, request.isActive, request.hasMfaEnabled, request.UserId.Value);
        userRepository.Update(existingUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"User {request.UserName} updated successfully.");
    }
}
#endregion
