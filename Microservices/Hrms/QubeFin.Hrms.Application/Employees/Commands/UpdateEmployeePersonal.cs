using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeePersonalCommand(
    Guid Id, /*string Code,*/ string? Salutation, string FirstName, string? MiddleName, string LastName, string? FatherName, string? MotherName, string? HusbandName,
    DateOnly DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup, string? DisablityType, string? MaritalStatus,
    Guid UserId, IFormFile? Photo = null, IFormFile? Signature = null
    ) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeePersonalCommandValidator : AbstractValidator<UpdateEmployeePersonalCommand>
{
    private static readonly string[] AllowedImageTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];

    public UpdateEmployeePersonalCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .Must(value => !string.IsNullOrWhiteSpace(value)
                && Regex.IsMatch(value, @"^[A-Za-z]+$")
                && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Please enter a valid First Name name.")
            .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
            .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .Matches("^[A-Za-z]{1,30}$")
            .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
        //RuleFor(x => x.BloodGroup).NotEmpty().WithMessage("Blood Group is required.");
        //RuleFor(x => x.Religion).NotEmpty().WithMessage("Religion is required.");
        RuleFor(x => x.Nationality).NotEmpty().WithMessage("Nationality is required.");
        RuleFor(x => x.Photo)
            .Must(BeAnImage!).When(x => x.Photo is not null)
            .WithMessage("Photo must be a JPG, PNG or WEBP image.");
        RuleFor(x => x.Signature)
            .Must(BeAnImage!).When(x => x.Signature is not null)
            .WithMessage("Signature must be a JPG, PNG or WEBP image.");
    }

    private static bool BeAnImage(IFormFile file) => AllowedImageTypes.Contains(file.ContentType?.ToLowerInvariant());
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeePersonalCommandHandler(
    IEmployeeRepository employeeRepository,
    IFileStorageRepository fileStorageRepository,
    IUnitOfWork unitOfWork,
    QubeFinDataContext context)
    : IRequestHandler<UpdateEmployeePersonalCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateEmployeePersonalCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        //var existingEmployee = await employeeRepository.GetExsitingEmployeeByCode(request.Id, request.Code);
        //if (existingEmployee)
        //{
        //    return new ValidationError("Employee already exist with same code.");
        //}

        employee.UpdatePersonalInfo(
            new PersonalInfo(employee.Code, request.Salutation, request.FirstName, request.MiddleName, request.LastName, request.FatherName, request.MotherName, request.HusbandName,
                request.DateOfBirth, request.Gender, request.Religion, request.Caste, request.Nationality, request.BloodGroup, request.DisablityType, request.MaritalStatus),
            request.UserId
            );

        await employeeRepository.UpdateAsync(employee);
        await SaveOrUpdateDocument(request.Id, CandidateJoiningDocumentCategory.Photo, "Passport Size Photo", request.Photo, request.UserId, cancellationToken);
        await SaveOrUpdateDocument(request.Id, CandidateJoiningDocumentCategory.Signature, "Signature", request.Signature, request.UserId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Employee personal information updated successfully for Name : {request.FirstName} {request.LastName}");
    }

    private async Task SaveOrUpdateDocument(Guid employeeId, string category, string documentName, IFormFile? file, Guid userId, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return;
        }

        await using var stream = file.OpenReadStream();
        var fileNo = await fileStorageRepository.UploadFileAsync(
            stream,
            file.FileName,
            file.ContentType ?? "application/octet-stream",
            cancellationToken).ConfigureAwait(false);

        var existing = await context.TblEmployeeDocuments
            .FirstOrDefaultAsync(m => m.EmployeeId == employeeId && m.DocumentCategory == category, cancellationToken);
        if (existing is null)
        {
            context.TblEmployeeDocuments.Add(new TblEmployeeDocument
            {
                Id = Guid.NewGuid(),
                DocumentCategory = category,
                DocumentName = documentName,
                FileName = file.FileName,
                FileNo = fileNo,
                EmployeeId = employeeId,
                UploadedBy = userId,
                UploadedOn = DateTime.Now
            });
            return;
        }

        existing.DocumentName = documentName;
        existing.FileName = file.FileName;
        existing.FileNo = fileNo;
        existing.UploadedBy = userId;
        existing.UploadedOn = DateTime.Now;
    }
}
#endregion
