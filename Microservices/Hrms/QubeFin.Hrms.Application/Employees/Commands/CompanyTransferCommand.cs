using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record CompanyTransferCommand(CompanyTransferRequest transfer, Guid loginUserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class CompanyTransferCommandValidator : AbstractValidator<CompanyTransferCommand>
{
    public CompanyTransferCommandValidator()
    {
        RuleFor(x => x.transfer).NotNull().WithMessage("Transfer details are required.");
        RuleFor(x => x.transfer.EmployeeId).NotEmpty().WithMessage("Employee is required.");
        RuleFor(x => x.transfer.CompanyId).NotEmpty().WithMessage("Company is required.");
        RuleFor(x => x.transfer.OrganisationUnitId).NotEmpty().WithMessage("Organisation Unit is required.");
        RuleFor(x => x.transfer.Designation).NotEmpty().WithMessage("Designation is required.");
        RuleFor(x => x.transfer.GradeId).NotEmpty().WithMessage("Grade is required.");
        RuleFor(x => x.transfer.GrossSalary).GreaterThanOrEqualTo(0).WithMessage("Gross Salary cannot be negative.");
        RuleFor(x => x.transfer.JoiningDate).NotEmpty().WithMessage("Joining Date is required.");
        RuleFor(x => x.transfer.SeparationDate).NotEmpty().WithMessage("Separation Date is required.");
        RuleFor(x => x.transfer.JoiningDate).GreaterThan(x => x.transfer.SeparationDate).WithMessage("Joining Date must be later than Separation Date.");
        RuleFor(x => x.transfer.OrganisationUnitId).NotEqual(x => x.transfer.EmployeeId).WithMessage("Organisation Unit cannot be the same as Employee.");
        RuleFor(x => x.transfer.Designation).NotEqual(x => x.transfer.GradeId.ToString()).WithMessage("Designation cannot be the same as Grade.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CompanyTransferCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork) : IRequestHandler<CompanyTransferCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CompanyTransferCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await employeeRepository.CompanyTransferEmployee(request.transfer.EmployeeId, request.transfer.CompanyId, request.transfer.OrganisationUnitId, request.transfer.Designation, request.transfer.GradeId, request.transfer.GrossSalary, request.transfer.JoiningDate, request.transfer.SeparationDate, request.loginUserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok($"Employee company transferred successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
#endregion
