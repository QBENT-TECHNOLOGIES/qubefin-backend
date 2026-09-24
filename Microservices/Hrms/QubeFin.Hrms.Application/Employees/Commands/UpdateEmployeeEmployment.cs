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
    public record UpdateEmployeeEmploymentCommand(
        Guid Id, List<EmploymentDetailRequest> Employments, Guid LastModifiedBy
        ) : IRequest<Result<string>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeEmploymentCommandValidator : AbstractValidator<UpdateEmployeeEmploymentCommand>
    {
        public UpdateEmployeeEmploymentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
            RuleFor(x => x.Employments).NotEmpty().WithMessage("At least one employment entry must be provided.");
            RuleForEach(x => x.Employments).SetValidator(new EmploymentDetailRequestValidator());
        }
    }
    public class EmploymentDetailRequestValidator : AbstractValidator<EmploymentDetailRequest>
    {
        public EmploymentDetailRequestValidator()
        {
            RuleFor(x => x.EmployerName).NotEmpty().WithMessage("Employer name is required.");
            RuleFor(x => x.Designation).NotEmpty().WithMessage("Designation is required.");
            RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate).WithMessage("To date cannot be earlier than from date.");
            RuleFor(x => x.LastDrawnSalary).GreaterThanOrEqualTo(0).WithMessage("Last drawn salary cannot be negative.");
        }
    }
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeEmploymentCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context, IFileStorageRepository fileStorageRepository)
        : IRequestHandler<UpdateEmployeeEmploymentCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(UpdateEmployeeEmploymentCommand request, CancellationToken cancellationToken)
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


                if (req.NocFile != null && req.NocFile.Length > 0)
                {
                    req.NocFileName = req.NocFile.FileName;
                    await using var stream = req.NocFile.OpenReadStream();
                    req.NocFileNo = await fileStorageRepository.UploadFileAsync(
                        stream,
                        req.NocFile.FileName,
                        req.NocFile.ContentType ?? "application/octet-stream",
                        cancellationToken).ConfigureAwait(false);
                }

                if (req.ExpCertFile != null && req.ExpCertFile.Length > 0)
                {
                    req.ExpCertFileName = req.ExpCertFile.FileName;
                    await using var stream = req.ExpCertFile.OpenReadStream();
                    req.ExpCertFileNo = await fileStorageRepository.UploadFileAsync(
                        stream,
                        req.ExpCertFile.FileName,
                        req.ExpCertFile.ContentType ?? "application/octet-stream",
                        cancellationToken).ConfigureAwait(false);
                }

                var employmentEntity = new TblEmployeeEmployment()
                {
                    Id = Guid.NewGuid()  ,
                    EmployeeId = request.Id,
                    EmployerName = req.EmployerName,
                    Designation = req.Designation,
                    FromDate = req.FromDate,
                    ToDate = req.ToDate,
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
            await employeeRepository.UpdateAsync(existingEmployee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Employee employment information updated successfully for Name : {existingEmployee.PersonalInfo.FirstName} {existingEmployee.PersonalInfo.LastName}");
        }
    }
    #endregion
}
