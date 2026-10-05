using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Auth.Application.Accounts.Model;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Auth.Application.Accounts.Commands;

#region --- COMMAND ---

public record PasswordResetCommand(ResetPassword Request, Guid UserId) : IRequest<Result<string>>;

#endregion
#region --- VALIDATOR ---

public class PasswordResetCommandValidator
    : AbstractValidator<PasswordResetCommand>
{
    public PasswordResetCommandValidator()
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request.Password).NotEmpty().WithMessage("password is required.");
        RuleFor(x => x.Request.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required.");
        RuleFor(x => x.Request.ConfirmPassword).Equal( x => x.Request.Password).WithMessage("Password and confirm password must match.");
    }
}

#endregion

#region --- HANDLER ---

internal sealed class PasswordResetCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<PasswordResetCommand, Result<string>>
{
    public async Task<Result<string>> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

       
        var user = await context.TblUsers.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return new RecordNotFoundError("User not found.");
        }
        if (req.Password != req.ConfirmPassword)
        {
            return Result.Fail("Password and confirm password do not match.");
        }

        var passwordHasher = new PasswordHasher<AppUser>();
        var appUser = new AppUser(user.UserName, req.Password);
        var hashedPassword = passwordHasher.HashPassword(appUser, req.Password);

        user.Password = hashedPassword;
        context.TblUsers.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Password reset successfully.");
    }
}

#endregion