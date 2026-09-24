using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

/// <summary>Which employee the candidate's joining information has been saved into, with what the candidate record
/// already holds. The joining form uses it to unlock the steps after Personal Info, resume where HR left off, and
/// prefill (and lock what Candidate Verification confirmed) on the employee steps.</summary>
public record GetCandidateJoiningInfoQuery(Guid CandidateId) : IRequest<Result<CandidateJoiningInfoResponse>>;

internal sealed class GetCandidateJoiningInfoQueryHandler(QubeFinDataContext context, IEmployeeRepository employeeRepository)
    : IRequestHandler<GetCandidateJoiningInfoQuery, Result<CandidateJoiningInfoResponse>>
{
    public async Task<Result<CandidateJoiningInfoResponse>> Handle(GetCandidateJoiningInfoQuery request, CancellationToken cancellationToken)
    {
        var candidate = await context.TblInterviewCandidates
            .AsNoTracking()
            .Include(x => x.PostedOrganizationUnit)
            .FirstOrDefaultAsync(x => x.Id == request.CandidateId, cancellationToken);

        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var employee = await context.TblEmployees
            .AsNoTracking()
            .Where(x => x.CandidateId == request.CandidateId)
            .Select(x => new { x.Id, x.Code })
            .FirstOrDefaultAsync(cancellationToken);

        AddressInfoResponse? address = null;
        if (candidate.AdministrativeUnitId is not null)
        {
            address = new AddressInfoResponse
            {
                HouseNo = candidate.HouseNo,
                RoadName = candidate.RoadName,
                LandMark = candidate.LandMark,
                AdministrativeUnitId = candidate.AdministrativeUnitId,
                PoliceStationId = candidate.PoliceStationId,
                PostOfficeId = candidate.PostOfficeId,
                PinCode = candidate.PinCode,
                AddressUnit = await employeeRepository.GetAdressUnit(candidate.AdministrativeUnitId.Value)
            };
        }

        // Tbl_Designation is a post at an office, so the interview post at the posted office is the designation.
        var designationId = candidate.PostedOrganizationUnitId is null ? null : await context.TblDesignations
            .AsNoTracking()
            .Where(d => d.PostId == candidate.InterviewPost && d.OrganizationUnitId == candidate.PostedOrganizationUnitId && d.IsActive)
            .Select(d => (Guid?)d.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return Result.Ok(new CandidateJoiningInfoResponse(
            CandidateId: candidate.Id,
            EmployeeId: employee?.Id,
            EmployeeCode: employee?.Code,
            MobileNo: candidate.MobileNo,
            IsMobileValidated: candidate.IsMobileValidated,
            Email: candidate.Email,
            AadharNumber: candidate.AadharNumber,
            IsAadharValidated: candidate.IsAadharValidated,
            VoterNumber: candidate.VoterNumber,
            IsVoterValidated: candidate.IsVoterValited,
            Pan: candidate.Pan,
            IsPanValidated: candidate.IsPanValidated,
            Uan: candidate.Uan,
            IsUanVerified: candidate.IsUanVerified,
            Address: address,
            CompanyId: candidate.CompanyId,
            OrganizationUnitTypeId: candidate.PostedOrganizationUnit?.OrganizationUnitTypeId,
            OrganizationUnitId: candidate.PostedOrganizationUnitId,
            DepartmentId: candidate.DepartmentId,
            DateOfJoining: candidate.DateOfJoining,
            DesignationId: designationId));
    }
}
