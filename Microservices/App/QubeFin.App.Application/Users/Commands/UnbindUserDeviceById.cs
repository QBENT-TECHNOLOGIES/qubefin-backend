using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record UnbindUserDeviceByIdCommand(Guid UserDeviceId, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UnbindUserDeviceByIdCommandValidator : AbstractValidator<UnbindUserDeviceByIdCommand>
{
    public UnbindUserDeviceByIdCommandValidator()
    {
        RuleFor(x => x.UserDeviceId).NotEmpty().WithMessage("User Device Id is required.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class UnbindUserDeviceByIdCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository) : IRequestHandler<UnbindUserDeviceByIdCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UnbindUserDeviceByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await userRepository.UnbindDeviceAsync(request.UserDeviceId, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Device unbinded successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to unbind device: {ex.Message}");
        }
    }
}
#endregion
