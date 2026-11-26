using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Members.Queries;

#region --- QUERY ---
public record GetMemberPersonalDetailsQuery(Guid Id) : IRequest<Result<MemberPersonalDetailsResponse>>;
#endregion

#region --- RESPONSE ---
public record MemberPersonalDetailsResponse(
    int BranchCode,
    string? BranchName,
    Guid? GroupId,
    string? GroupCode,
    string? GroupName,
    string? CreditOfficerName,
    string? EmployeeCode,
    string? Code,
    string? Salutation,
    string FirstName,
    string? MiddleName,
    string? LastName,
    string FullName,
    string? GuardianName,
    string? MotherName,
    string? SpouseName,
    DateOnly DateOfBirth,
    string? Gender,
    string? Religion,
    string? Caste,
    string? Nationality,
    string? MaritalStatus,
    string? EducationalQualification,
    string MobileNo,
    string? AlternateMobileNo
);
#endregion

#region --- HANDLER ---
internal sealed class GetMemberPersonalDetailsQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetMemberPersonalDetailsQuery, Result<MemberPersonalDetailsResponse>>
{
    public async Task<Result<MemberPersonalDetailsResponse>> Handle(
        GetMemberPersonalDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var member = await context.TblMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (member is null)
        {
            return new ValidationError("Member does not exist with the given id.");
        }

        string? groupCode = null;
        string? groupName = null;

        if (member.GroupId.HasValue && member.GroupId.Value != Guid.Empty)
        {
            var group = await context.TblGroups
                .AsNoTracking()
                .Where(g => g.Id == member.GroupId.Value)
                .Select(g => new { g.Code, g.Name })
                .FirstOrDefaultAsync(cancellationToken);

            groupCode = group?.Code;
            groupName = group?.Name;
        }

        string? branchName = null;
        string? creditOfficerName = null;
        string? employeeCode = null;
        Guid? designationId = member.DesignationId;

        if (designationId.HasValue && designationId.Value != Guid.Empty)
        {
            var employeeDesignation = await context.TblEmployeeDesignations
                .AsNoTracking()
                .Include(ed => ed.Employee)
                .ThenInclude(e => e.OrganizationUnit)
                .Where(ed => ed.DesignationId == designationId.Value)
                .OrderByDescending(ed => ed.EffectiveTo == null)
                .ThenByDescending(ed => ed.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            branchName = employeeDesignation?.Employee?.OrganizationUnit?.Name;
            creditOfficerName = employeeDesignation?.Employee?.FullName;
            employeeCode = employeeDesignation?.Employee?.Code;
        }

        var response = new MemberPersonalDetailsResponse(
            member.BranchCode,
            branchName,
            member.GroupId,
            groupCode,
            groupName,
            creditOfficerName,
            employeeCode,
            member.Code,
            member.Salutaion,
            member.FirstName,
            member.MiddleName,
            member.LastName,
            member.FullName,
            member.GuardianName,
            member.MotherName,
            member.SpouseName,
            member.DateOfBirth,
            member.Gender,
            member.Religion,
            member.Caste,
            member.Nationality,
            member.MaritalStatus,
            member.Qualification,
            member.MobileNo,
            member.AlternateMobileNo
        );

        return Result.Ok(response);
    }
}
#endregion