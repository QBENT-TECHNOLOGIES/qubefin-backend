using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Hrms.Application.Employees.Commands
{
    #region --- COMMAND ---
    public record UpdateEmployeeDependentNomineeCommand(
        Guid Id, List<DependentNomineeDetailRequest> DependentNominees, Guid LastModifiedBy
        ) : IRequest<Result<string>>;
    #endregion

    #region --- VALIDATION ---
    public class UpdateEmployeeDependentNomineeCommandValidator : AbstractValidator<UpdateEmployeeDependentNomineeCommand>
    {
        public UpdateEmployeeDependentNomineeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
            RuleFor(x => x.LastModifiedBy).NotEmpty().WithMessage("Modifier User ID is required.");
            RuleFor(x => x.DependentNominees).NotEmpty().WithMessage("At least one dependent/nominee entry must be provided.");
            RuleForEach(x => x.DependentNominees).SetValidator(new DependentNomineeDetailRequestValidator());
        }
    }
    public class DependentNomineeDetailRequestValidator : AbstractValidator<DependentNomineeDetailRequest>
    {
        public DependentNomineeDetailRequestValidator()
        {
            RuleFor(x => x.NomineeName).NotEmpty().WithMessage("Nominee name is required.");
            RuleFor(x => x.RelationWithInsuredPerson).NotEmpty().WithMessage("Relation with insured person is required.");
            RuleFor(x => x.Age).GreaterThanOrEqualTo(0).When(x => x.Age.HasValue).WithMessage("Age cannot be negative.");
        }
    }
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeDependentNomineeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeDependentNomineeCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UpdateEmployeeDependentNomineeCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }

            var updatedNomineeEntityList = request.DependentNominees.Select(req => new TblEmployeeDependentNominee
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.Id,
                NomineeName = req.NomineeName,
                RelationWithInsuredPerson = req.RelationWithInsuredPerson,
                DateOfBirth = req.DateOfBirth,
                Age = req.Age,
                UhidAbhaNumber = req.UhidAbhaNumber,
                AbhaAddress = req.AbhaAddress,
                Uan = req.Uan,
                AadharNumber = req.AadharNumber,
                VoterIdnumber = req.VoterIdnumber,
                IsResidingWithIp = req.IsResidingWithIp
            }).ToList();

            var existingNominees = await context.TblEmployeeDependentNominees.Where(m => m.EmployeeId == request.Id).ToListAsync(cancellationToken: cancellationToken);
            if (existingNominees != null && existingNominees.Count() > 0)
            {
                context.TblEmployeeDependentNominees.RemoveRange(existingNominees);
            }
            context.TblEmployeeDependentNominees.AddRange(updatedNomineeEntityList);
            existingEmployee.SetModified(request.LastModifiedBy);
            await employeeRepository.UpdateAsync(existingEmployee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Employee dependent/nominee information updated successfully for Name : {existingEmployee.PersonalInfo.FirstName} {existingEmployee.PersonalInfo.LastName}");
        }
    }
    #endregion
}
