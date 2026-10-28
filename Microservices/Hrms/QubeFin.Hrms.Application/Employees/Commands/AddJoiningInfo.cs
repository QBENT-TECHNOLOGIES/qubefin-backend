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
            var referenceNo = existingReferenceNo ?? await GenerateReferenceNoAsync(cancellationToken);

            await employeeRepository.SaveJoiningInfoAsync(joiningInfo.EmployeeId, referenceNo, joiningInfo.OfferLetterDate, joiningInfo.MonthlyCtc, request.UserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return existingReferenceNo is null
                ? Result.Ok($"Joining information created successfully. Reference No: {referenceNo}.")
                : Result.Ok($"Joining information updated successfully. Reference No: {referenceNo}.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    private async Task<string> GenerateReferenceNoAsync(CancellationToken cancellationToken)
    {
        var countThisYear = await employeeRepository.CountJoiningInfoCreatedInYearAsync(cancellationToken);
        var year = DateTime.UtcNow.Year;
        return $"WGRW/JOIN/{year}/{countThisYear + 1:D4}";
    }
}

#endregion
