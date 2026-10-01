using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record UnbindUserDeviceByIdCommand(Guid DeviceId, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UnbindUserDeviceByIdCommandValidator : AbstractValidator<UnbindUserDeviceByIdCommand>
{
    public UnbindUserDeviceByIdCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty().WithMessage("Device Id is required.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class UnbindUserDeviceByIdCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork, IUserRepository userRepository) : IRequestHandler<UnbindUserDeviceByIdCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UnbindUserDeviceByIdCommand request, CancellationToken cancellationToken)
    {
        
        var userDeviceEntity = await context.TblUserDevices.FirstOrDefaultAsync(m => m.Id == request.DeviceId);
        if (userDeviceEntity == null)
        {
            return Result.Fail($"Device Not Found.");
        }
        await userRepository.UnbindDeviceAsync(userDeviceEntity.Id, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Device unbinded successfully.");
    }
}
#endregion
