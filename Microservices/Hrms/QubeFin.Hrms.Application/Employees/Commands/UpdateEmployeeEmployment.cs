using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record UpdateEmployeeEmploymentCommand(
        Guid Id, List<EmploymentDetailRequest> Employments, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeEmploymentResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeEmploymentCommandValidator : AbstractValidator<UpdateEmployeeEmploymentCommand>
    {
        public UpdateEmployeeEmploymentCommandValidator()
        {
            //RuleFor(x => x.FirstName)
            //    .Must(value => !string.IsNullOrWhiteSpace(value)
            //        && Regex.IsMatch(value, @"^[A-Za-z]+$")
            //        && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
            //    .WithMessage("Please enter a valid First Name name.")
            //    .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
            //    .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
            //RuleFor(x => x.LastName)
            //    .NotEmpty()
            //    .Matches("^[A-Za-z]{3,30}$")
            //    .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            
        }
    }
    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeEmploymentResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeEmploymentCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeEmploymentCommand, Result<UpdateEmployeeEmploymentResponse>>
    {
        public async Task<Result<UpdateEmployeeEmploymentResponse>> Handle(UpdateEmployeeEmploymentCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }
            // 2. Project incoming requests directly into domain entity shapes
            var updatedEmploymentEntityList = new List<EmployeeEmployment>();

            var orderByQualifications = request.Employments.OrderBy(m => m.Sequence).ToList();
            for (int i = 0; i < orderByQualifications.Count; i++)
            {
                var req = orderByQualifications[i];
                int sequenceValue = i + 1;
                //int sequenceValue = i + 1;

                var employmentEntity = new EmployeeEmployment(
                    id: Guid.NewGuid(),
                    employerName: req.EmployerName,
                    designation: req.Designation,
                    fromDate: req.FromDate,
                    toDate: req.ToDate,
                    lastDrawnSalary: req.LastDrawnSalary,
                    jobTitle: req.JobTitle,
                    nocFileName: req.NocFileName,
                    nocFileNo: req.NocFileNo,
                    expCertFileName: req.ExpCertFileName,
                    expCertFileNo: req.ExpCertFileNo,
                    employeeId: request.Id,
                    sequence: sequenceValue, // Integer index from your loop (e.g., i + 1)
                    createdBy: request.LastModifiedBy
                );

                updatedEmploymentEntityList.Add(employmentEntity);
            }

            // 3. Atomically overwrite old items and explicitly log modifications
            existingEmployee.ReplaceEmployments(updatedEmploymentEntityList);

            await employeeRepository.UpdateAsync(existingEmployee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeEmploymentResponse(true));

        }
    }
    #endregion
}
