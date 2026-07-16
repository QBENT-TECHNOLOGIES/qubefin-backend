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
    public record UpdateEmployeeQualificationCommand(
            Guid Id, IReadOnlyList<QualificationRequest> Qualifications, Guid LastModifiedBy
        ) : IRequest<Result<UpdateEmployeeQualificationResponse>>;
    #endregion
    #region --- VALIDATION ---
    public class UpdateEmployeeQualificationCommandValidator : AbstractValidator<UpdateEmployeeQualificationCommand>
    {
        public UpdateEmployeeQualificationCommandValidator()
        {
            RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.LastModifiedBy)
                .NotEmpty().WithMessage("Modifier User ID is required.");

            // Validates that the list itself is not completely empty
            RuleFor(x => x.Qualifications)
                .NotEmpty().WithMessage("At least one qualification entry must be provided.");

            // Loops through the array and validates each record automatically
            RuleForEach(x => x.Qualifications)
                .SetValidator(new QualificationRequestValidator());

        }
    }
    public class QualificationRequestValidator : AbstractValidator<QualificationRequest>
    {
        public QualificationRequestValidator()
        {
            RuleFor(x => x.AcademicStream)
                .NotEmpty().WithMessage("Academic stream is required.")
                .MaximumLength(100).WithMessage("Academic stream cannot exceed 100 characters.");

            RuleFor(x => x.UniversityOrBoard)
                .NotEmpty().WithMessage("University or Board name is required.")
                .MaximumLength(150).WithMessage("University or Board name cannot exceed 150 characters.");

            RuleFor(x => x.SchoolOrCollege)
                .NotEmpty().WithMessage("School or College name is required.")
                .MaximumLength(150).WithMessage("School or College name cannot exceed 150 characters.");

            RuleFor(x => x.YearOfPassing)
                .NotEmpty().WithMessage("Year of passing is required.")
                .InclusiveBetween(1950, DateTime.UtcNow.Year)
                .WithMessage($"Year of passing must be a valid year between 1950 and {DateTime.UtcNow.Year}.");

            RuleFor(x => x.GradeOrMarks)
                .NotEmpty().WithMessage("Grade or Marks summary is required.")
                .MaximumLength(20).WithMessage("Grade or Marks entry cannot exceed 20 characters.");
        }
    }

    #endregion

    #region --- RESPONSE ---
    public record UpdateEmployeeQualificationResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class UpdateEmployeeQualificationCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, QubeFinDataContext context)
        : IRequestHandler<UpdateEmployeeQualificationCommand, Result<UpdateEmployeeQualificationResponse>>
    {
        public async Task<Result<UpdateEmployeeQualificationResponse>> Handle(UpdateEmployeeQualificationCommand request, CancellationToken cancellationToken)
        {
            var existingEmployee = await employeeRepository.GetByIdAsync(request.Id);
            if (existingEmployee == null)
            {
                return new ValidationError("Employee not exist given id.");
            }
            // 2. Project incoming requests directly into domain entity shapes
            var updatedQualificationsList = new List<EmployeeQualification>();

            var orderByQualifications = request.Qualifications.OrderBy(m => m.Sequence).ToList();
            for (int i = 0; i < orderByQualifications.Count; i++)
            {
                var req = orderByQualifications[i];
                int sequenceValue = i + 1;

                var qualificationEntity = new EmployeeQualification(
                    Guid.NewGuid(),
                    req.AcademicStream,
                    req.Specialization,
                    req.YearOfPassing,
                    req.UniversityOrBoard,
                    req.SchoolOrCollege,
                    req.GradeOrMarks,
                    req.DocFileName,
                    req.DocFileNo,
                    request.Id,
                    sequenceValue
                );
                updatedQualificationsList.Add(qualificationEntity);
            }

            // 3. Atomically overwrite old items and explicitly log modifications
            existingEmployee.ReplaceQualifications(updatedQualificationsList);

            await employeeRepository.UpdateAsync(existingEmployee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateEmployeeQualificationResponse(true));


        }
    }
    #endregion
}
