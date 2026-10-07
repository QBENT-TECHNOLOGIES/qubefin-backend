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
public record PasswordResetCommand(ResetPassword resetRequest, Guid UserId) : IRequest<Result<string>>;
#endregion
#region --- VALIDATOR ---
public class PasswordResetCommandValidator : AbstractValidator<PasswordResetCommand>
{
    public PasswordResetCommandValidator()
    {
        RuleFor(x => x.resetRequest).NotNull();
        RuleFor(x => x.resetRequest.Password).NotEmpty().WithMessage("password is required.");
        RuleFor(x => x.resetRequest.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.resetRequest.Password).WithMessage("Password and confirm password must match.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class PasswordResetCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<PasswordResetCommand, Result<string>>
{
    public async Task<Result<string>> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        if (request.resetRequest.Password.Trim() != request.resetRequest.ConfirmPassword.Trim())
        {
            return Result.Fail("Password and confirm password do not match.");
        }
        var user = await context.TblUsers.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            return new RecordNotFoundError("User not found.");
        }
        var passwordHasher = new PasswordHasher<AppUser>();
        var hashedPassword = passwordHasher.HashPassword(new AppUser(user.UserName, request.resetRequest.Password), request.resetRequest.Password);

        user.Password = hashedPassword;
        context.TblUsers.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Password reset successfully for user {user.UserName}");
    }
}
#endregion