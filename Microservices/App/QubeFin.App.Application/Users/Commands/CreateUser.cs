using FluentResults;
using MediatR;
using OtpNet;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record CreateUserCommand(string UserName, string Password, Guid? EmployeeId) : IRequest<Result<CreateUserResponse>>;
#endregion

#region --- RESPONSE ---
public record CreateUserResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var mfaSecretKey32 = Base32Encoding.ToString(secretKey);
        string hashedPassword = await userRepository.HashPasswordAsync(request.UserName, request.Password);

        var user = User.Create(Guid.NewGuid(), request.UserName, hashedPassword, request.EmployeeId, mfaSecretKey32, Guid.Empty);
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateUserResponse(true));
    }
}
#endregion