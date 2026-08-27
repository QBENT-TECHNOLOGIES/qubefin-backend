using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record UnbindUserDeviceCommand(string UserName) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UnbindUserDeviceCommandValidator : AbstractValidator<UnbindUserDeviceCommand>
{
    public UnbindUserDeviceCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class UnbindUserDeviceCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<UnbindUserDeviceCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UnbindUserDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await context.TblUsers.FirstOrDefaultAsync(m => m.UserName.Trim() == request.UserName.Trim());

        if (user == null)
        {
            return Result.Fail($"User {request.UserName} not found.");
        }
        var userDeviceEntity = await context.TblUserDevices.FirstOrDefaultAsync(m => m.UserId == user.Id);
        if (userDeviceEntity == null)
        {
            return Result.Fail($"This User has no registered devices yet.");
        }
        context.TblUserDevices.Remove(userDeviceEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Device unbinded successfully.");
    }
}
#endregion
