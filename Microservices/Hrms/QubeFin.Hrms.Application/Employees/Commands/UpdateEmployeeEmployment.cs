using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
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
            var updatedEmploymentEntityList = new List<TblEmployeeEmployment>();

            var orderByQualifications = request.Employments.OrderBy(m => m.Sequence).ToList();
            for (int i = 0; i < orderByQualifications.Count; i++)
            {
                var req = orderByQualifications[i];
                int sequenceValue = i + 1;
                //int sequenceValue = i + 1;

                var employmentEntity = new TblEmployeeEmployment(){
                    Id = req.Id,
                    EmployeeId = request.Id,
                    EmployerName = req.EmployerName,
                    Designation = req.Designation,
                    FromDate = DateOnly.FromDateTime(req.FromDate),
                    ToDate = DateOnly.FromDateTime(req.ToDate),
                    LastDrawnSalary = req.LastDrawnSalary,
                    JobTitle = req.JobTitle,
                    NocFileName = req.NocFileName,
                    NocFileNo = req.NocFileNo,
                    ExpCertFileName = req.ExpCertFileName,
                    ExpCertFileNo = req.ExpCertFileNo,
                    Sequence = sequenceValue,
                    CreatedBy = request.LastModifiedBy,
                    CreatedOn = DateTime.Now
                };

                updatedEmploymentEntityList.Add(employmentEntity);
            }
            var emp = await context.TblEmployeeEmployments.Where(m => m.EmployeeId == request.Id).ToListAsync(cancellationToken: cancellationToken);
            if (emp != null && emp.Count() > 0)
            {
                context.TblEmployeeEmployments.RemoveRange(emp);
            }
            context.TblEmployeeEmployments.AddRange(updatedEmploymentEntityList);
            existingEmployee.SetModified(request.LastModifiedBy);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeEmploymentResponse(true));

        }
    }
    #endregion
}
