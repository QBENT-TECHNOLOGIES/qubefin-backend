using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Auth.Application.Accounts.Model;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---
public record ChangePasswordCommand(ChangePasswordRequest Request, Guid userId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Password).NotEmpty().WithMessage("Current password is required.");
        RuleFor(x => x.Request.NewPassword).NotEmpty().WithMessage("New password is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class ChangePasswordCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        var userEntity = await context.TblUsers.FirstOrDefaultAsync(m => m.Id == request.userId, cancellationToken);
        if (userEntity is null)
        {
            return new RecordNotFoundError($"User not found.");
        }

        var passwordHasher = new PasswordHasher<AppUser>();
        var appUserForVerify = new AppUser(userEntity.UserName, req.Password);
        var verifyResult = passwordHasher.VerifyHashedPassword(appUserForVerify, userEntity.Password, req.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return new ValidationError("Current password is incorrect.");
        }

        var hashedNew = passwordHasher.HashPassword(new AppUser(userEntity.UserName, req.NewPassword), req.NewPassword);
        userEntity.Password = hashedNew;
        context.TblUsers.Update(userEntity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Password changed successfully.");
    }
}
#endregion
