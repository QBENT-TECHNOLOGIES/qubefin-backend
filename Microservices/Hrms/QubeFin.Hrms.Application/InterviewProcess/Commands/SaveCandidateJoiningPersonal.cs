using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Commands;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

#region --- COMMAND ---
/// <summary>Personal step of the candidate joining form. The first save creates the employee from the joining
/// information and links it to the candidate (Tbl_Employee.CandidateId); later saves update that
/// employee. The photo and signature are stored as Tbl_EmployeeDocument rows.</summary>
public record SaveCandidateJoiningPersonalCommand(Guid CandidateId, CandidateJoiningPersonalRequest Personal, Guid UserId)
    : IRequest<Result<SaveCandidateJoiningPersonalResponse>>;
#endregion

#region --- VALIDATION ---
public class SaveCandidateJoiningPersonalCommandValidator : AbstractValidator<SaveCandidateJoiningPersonalCommand>
{
    private static readonly string[] AllowedImageTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];

    public SaveCandidateJoiningPersonalCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Personal.Photo)
            .Must(BeAnImage!).When(x => x.Personal.Photo is not null)
            .WithMessage("Photo must be a JPG, PNG or WEBP image.");
        RuleFor(x => x.Personal.Signature)
            .Must(BeAnImage!).When(x => x.Personal.Signature is not null)
            .WithMessage("Signature must be a JPG, PNG or WEBP image.");
    }

    private static bool BeAnImage(IFormFile file) => AllowedImageTypes.Contains(file.ContentType?.ToLowerInvariant());
}
#endregion

#region --- RESPONSE ---
public record SaveCandidateJoiningPersonalResponse(Guid EmployeeId, string? Message);
#endregion

#region --- HANDLER ---
internal sealed class SaveCandidateJoiningPersonalCommandHandler(
    ICandidateRepository candidateRepository,
    IFileStorageRepository fileStorageRepository,
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork,
    QubeFinDataContext context,
    ISender sender) : IRequestHandler<SaveCandidateJoiningPersonalCommand, Result<SaveCandidateJoiningPersonalResponse>>
{
    public async Task<Result<SaveCandidateJoiningPersonalResponse>> Handle(SaveCandidateJoiningPersonalCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var p = request.Personal;
        var existingEmployeeId = await candidateRepository.GetEmployeeIdAsync(request.CandidateId, cancellationToken);
        Guid employeeId;
        string? message;

        if (existingEmployeeId is null)
        {
            var existingEmployee = await employeeRepository.GetExsitingEmployeeByCode(null, p.Code);
            if (existingEmployee)
            {
                return new ValidationError("Employee already exist with same code.");
            }

            employeeId = Guid.NewGuid();
            var employee = Employee.Create(
                employeeId,
                p.Code,
                new PersonalInfo(p.Code, p.Salutation, p.FirstName, p.MiddleName, p.LastName, p.FatherName, p.MotherName, p.HusbandName,
                    p.DateOfBirth, p.Gender, p.Religion, p.Caste, p.Nationality, p.BloodGroup, p.DisablityType, p.MaritalStatus),
                new OfficialInfo(),
                new ContactInfo(),
                new AddressInfo(),
                new AddressInfo(),
                new PayrollInfo(),
                request.UserId,
                request.CandidateId);

            // Saved together with the photo/signature rows below.
            await employeeRepository.AddAsync(employee);
            message = $"Employee created successfully with Name : {p.FirstName} {p.LastName}";
        }
        else
        {
            employeeId = existingEmployeeId.Value;
            var updated = await sender.Send(new UpdateEmployeePersonalCommand(employeeId, p.Code, p.Salutation, p.FirstName, p.MiddleName, p.LastName, p.FatherName, p.MotherName,
                p.HusbandName, p.DateOfBirth, p.Gender, p.Religion, p.Caste, p.Nationality, p.BloodGroup, p.DisablityType, p.MaritalStatus, request.UserId), cancellationToken);
            if (updated.IsFailed)
            {
                return Result.Fail(updated.Errors);
            }

            message = updated.Value;
            // UpdateEmployeePersonalCommand saved through the same context - drop what it left tracked.
            context.ChangeTracker.Clear();
        }

        await ReplaceDocument(employeeId, CandidateJoiningDocumentCategory.Photo, "Passport Size Photo", p.Photo, request.UserId, cancellationToken);
        await ReplaceDocument(employeeId, CandidateJoiningDocumentCategory.Signature, "Signature", p.Signature, request.UserId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new SaveCandidateJoiningPersonalResponse(employeeId, message));
    }

    /// <summary>Stores a newly picked photo/signature, replacing the previous one. Nothing changes when no file is sent.</summary>
    private async Task ReplaceDocument(Guid employeeId, string category, string documentName, IFormFile? file, Guid userId, CancellationToken cancellationToken)
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
            .Where(m => m.EmployeeId == employeeId && m.DocumentCategory == category)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            context.TblEmployeeDocuments.RemoveRange(existing);
        }

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
    }
}
#endregion
