using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

/// <summary>Personal step of the joining form. Reads the linked employee's personal info, photo and signature;
/// before the employee exists it falls back to what the candidate record already holds.</summary>
public record GetCandidateJoiningPersonalQuery(Guid CandidateId) : IRequest<Result<CandidateJoiningPersonalResponse>>;

internal sealed class GetCandidateJoiningPersonalQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository)
    : IRequestHandler<GetCandidateJoiningPersonalQuery, Result<CandidateJoiningPersonalResponse>>
{
    public async Task<Result<CandidateJoiningPersonalResponse>> Handle(GetCandidateJoiningPersonalQuery request, CancellationToken cancellationToken)
    {
        var candidate = await context.TblInterviewCandidates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CandidateId, cancellationToken);

        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var employeeId = await context.TblEmployees
            .AsNoTracking()
            .Where(x => x.CandidateId == request.CandidateId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (employeeId is null)
        {
            return Result.Ok(new CandidateJoiningPersonalResponse(
                EmployeeId: null,
                Code: null,
                Salutation: null,
                FirstName: candidate.FirstName,
                MiddleName: candidate.MiddleName,
                LastName: candidate.LastName,
                FatherName: candidate.FatherName,
                MotherName: null,
                HusbandName: null,
                DateOfBirth: null,
                Gender: candidate.Gender,
                Religion: null,
                Caste: null,
                Nationality: null,
                BloodGroup: null,
                DisablityType: null,
                MaritalStatus: null,
                PhotoFileName: null,
                PhotoFileUrl: null,
                SignatureFileName: null,
                SignatureFileUrl: null));
        }

        var employee = await context.TblEmployees
            .AsNoTracking()
            .Include(m => m.TblEmployeeDocuments)
            .FirstOrDefaultAsync(m => m.Id == employeeId, cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError("Employee linked to the candidate was not found.");
        }

        var photo = employee.TblEmployeeDocuments.FirstOrDefault(m => m.DocumentCategory == CandidateJoiningDocumentCategory.Photo);
        var signature = employee.TblEmployeeDocuments.FirstOrDefault(m => m.DocumentCategory == CandidateJoiningDocumentCategory.Signature);

        return Result.Ok(new CandidateJoiningPersonalResponse(
            EmployeeId: employee.Id,
            Code: employee.Code,
            Salutation: employee.Salutation,
            FirstName: employee.FirstName,
            MiddleName: employee.MiddleName,
            LastName: employee.LastName,
            FatherName: employee.FatherName,
            MotherName: employee.MotherName,
            HusbandName: employee.HusbandName,
            DateOfBirth: employee.DateOfBirth,
            Gender: employee.Gender,
            Religion: employee.Religion,
            Caste: employee.Caste,
            Nationality: employee.Nationality,
            BloodGroup: employee.BloodGroup,
            DisablityType: employee.DisablityType,
            MaritalStatus: !string.IsNullOrWhiteSpace(employee.MaritalStatus) ? employee.MaritalStatus?.Trim() : null,
            PhotoFileName: photo?.FileName,
            PhotoFileUrl: await FileUrl(photo?.FileNo, cancellationToken),
            SignatureFileName: signature?.FileName,
            SignatureFileUrl: await FileUrl(signature?.FileNo, cancellationToken)));
    }

    private async Task<string?> FileUrl(string? fileNo, CancellationToken cancellationToken)
    {
        return string.IsNullOrEmpty(fileNo) ? null : await fileStorageRepository.GetFileUrlAsync(fileNo, cancellationToken);
    }
}
