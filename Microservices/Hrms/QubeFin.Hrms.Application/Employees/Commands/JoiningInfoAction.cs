using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---

public record JoiningInfoActionCommand(JoiningInfoActionRequest Action, Guid UserId) : IRequest<Result<string>>;

#endregion

#region --- VALIDATION ---
public class JoiningInfoActionCommandValidator : AbstractValidator<JoiningInfoActionCommand>
{
    public JoiningInfoActionCommandValidator()
    {
        RuleFor(x => x.Action.JoiningInfoId).NotEmpty().WithMessage("Joining info is required.");
    }
}
#endregion

#region --- HANDLER ---

internal sealed class JoiningInfoActionCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork) : IRequestHandler<JoiningInfoActionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(JoiningInfoActionCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        try
        {
            var status = request.Action.IsApproved ? JoiningInfoApprovalStatus.Approved : JoiningInfoApprovalStatus.Rejected;

            await employeeRepository.SetJoiningInfoApprovalAsync(request.Action.JoiningInfoId, status, request.Action.IsApproved, request.UserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok($"Joining information {status.ToLowerInvariant()} successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

#endregion
