using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Commands;

#region --- COMMAND ---
public record UpdateFcmInstallationIdCommand(Guid UserId, string FcmInstallationId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateFcmInstallationIdCommandValidator : AbstractValidator<UpdateFcmInstallationIdCommand>
{
    public UpdateFcmInstallationIdCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required.");
        RuleFor(x => x.FcmInstallationId).NotEmpty().WithMessage("Fcm installation id is required.");
        RuleFor(x => x.FcmInstallationId).MaximumLength(200).WithMessage("Fcm installation id must not exceed 200 characters.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class UpdateFcmInstallationIdCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFcmInstallationIdCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateFcmInstallationIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await userRepository.UpdateFcmInstallationIdAsync(request.UserId, request.FcmInstallationId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Fcm installation id updated successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to update fcm installation id: {ex.Message}");
        }
    }
}
#endregion
