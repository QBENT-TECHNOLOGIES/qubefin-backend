using FluentResults;
using FluentValidation;
using MediatR;
using OtpNet;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;
using System.Text.RegularExpressions;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record CreateUserCommand(string UserName, string Password, Guid EmployeeId, Guid? UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Employee id is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUserName = await userRepository.GetExsitingUserByUserName(null, request.UserName);
        if (existingUserName)
        {
            return new ValidationError("Employee already exist with same code.");
        }

        var exsitingEmployeeById = await userRepository.GetExsitingEmployeeById(request.EmployeeId);
        if (exsitingEmployeeById)
        {
            return new ValidationError("A user is already registered for this employee.");
        }

        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var mfaSecretKey32 = Base32Encoding.ToString(secretKey);
        string hashedPassword = await userRepository.HashPasswordAsync(request.UserName, request.Password);

        var user = User.Create(Guid.NewGuid(), request.UserName, hashedPassword, request.EmployeeId, mfaSecretKey32, request.UserId.Value);
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"User {request.UserName} created successfully.");
    }
}
#endregion