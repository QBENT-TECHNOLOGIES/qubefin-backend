using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---

public record AddJoiningInfoCommand(JoiningInfoRequest JoiningInfo, Guid UserId) : IRequest<Result<string>>;

#endregion

#region --- VALIDATION ---
public class AddJoiningInfoCommandValidator : AbstractValidator<AddJoiningInfoCommand>
{
    public AddJoiningInfoCommandValidator()
    {
        RuleFor(x => x.JoiningInfo.EmployeeId).NotEmpty().WithMessage("Employee is required.");
        RuleFor(x => x.JoiningInfo.OfferLetterDate).NotEmpty().WithMessage("Offer Letter Date is required.");
        RuleFor(x => x.JoiningInfo.MonthlyCtc).GreaterThan(0).WithMessage("Monthly CTC must be greater than zero.");
        RuleFor(x => x.JoiningInfo.ReferenceNo).NotEmpty().WithMessage("Reference No. is required.");
    }
}
#endregion

#region --- HANDLER ---

internal sealed class AddJoiningInfoCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddJoiningInfoCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddJoiningInfoCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        try
        {
            var joiningInfo = request.JoiningInfo;
            var existingReferenceNo = await employeeRepository.GetJoiningInfoReferenceNoAsync(joiningInfo.EmployeeId, cancellationToken);

            await employeeRepository.SaveJoiningInfoAsync(joiningInfo.EmployeeId, joiningInfo.ReferenceNo, joiningInfo.OfferLetterDate, joiningInfo.MonthlyCtc, request.UserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return existingReferenceNo is null
                ? Result.Ok($"Joining information created successfully. Reference No: {request.JoiningInfo.ReferenceNo}.")
                : Result.Ok($"Joining information updated successfully. Reference No: {request.JoiningInfo.ReferenceNo}.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

#endregion
