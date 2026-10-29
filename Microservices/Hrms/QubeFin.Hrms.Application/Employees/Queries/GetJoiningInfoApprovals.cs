using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Application.LeaveApproval.Models;
using QubeFin.Persistence;
namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetJoiningInfoApprovalsQuery(Guid EmployeeId, LeaveApprovalSearchRequest SearchParam) : IRequest<GetJoiningInfoApprovalsResponse>;
#endregion

#region --- VALIDATION ---
public class GetJoiningInfoApprovalsQueryValidator : AbstractValidator<GetJoiningInfoApprovalsQuery>
{
    public GetJoiningInfoApprovalsQueryValidator()
    {
        RuleFor(x => x.SearchParam.PageSize).GreaterThan(0).WithMessage("Page Size is required.");
        RuleFor(x => x.SearchParam.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page Index is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetJoiningInfoApprovalsResponse(IReadOnlyList<JoiningInfoApprovalListItem> Results, int TotalRecords, bool HasAccess);
#endregion

#region --- HANDLER ---
internal sealed class GetJoiningInfoApprovalsQueryHandler(QubeFinDataContext context, IConfiguration configuration) : IRequestHandler<GetJoiningInfoApprovalsQuery, GetJoiningInfoApprovalsResponse>
{
    public async Task<GetJoiningInfoApprovalsResponse> Handle(GetJoiningInfoApprovalsQuery request, CancellationToken cancellationToken)
    {
        var searchParam = request.SearchParam;

        var isMd = Guid.TryParse(configuration["MDPost"], out var mdPostId)
            && await context.TblDesignations.AnyAsync(d => d.PostId == mdPostId && d.TblEmployeeDesignations.Any(ed => ed.EmployeeId == request.EmployeeId && ed.EffectiveTo == null), cancellationToken);

        var mdCompanyId = isMd
            ? await context.TblEmployees.AsNoTracking().Where(e => e.Id == request.EmployeeId).Select(e => e.CompanyId).FirstOrDefaultAsync(cancellationToken)
            : null;

        if (mdCompanyId is null)
        {
            return new GetJoiningInfoApprovalsResponse([], 0, false);
        }

        var query = context.TblJoiningInfos
            .AsNoTracking()
            .Where(j => j.Employee.CompanyId == mdCompanyId)
            .Where(j => j.ApprovalStatus == null || j.ApprovalStatus == JoiningInfoApprovalStatus.Pending);

        if (searchParam.SearchEmployeeId.HasValue)
            query = query.Where(j => j.EmployeeId == searchParam.SearchEmployeeId.Value);

        if (searchParam.FromDate.HasValue)
            query = query.Where(j => j.Employee.JoiningDate >= searchParam.FromDate.Value);

        if (searchParam.ToDate.HasValue)
            query = query.Where(j => j.Employee.JoiningDate <= searchParam.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(searchParam.SearchText))
            query = query.Where(j => j.Employee.FullName.Contains(searchParam.SearchText) || j.ReferenceNo.Contains(searchParam.SearchText));

        var totalRecords = await query.CountAsync(cancellationToken);

        var rows = await ApplySort(query, searchParam.SortOn, searchParam.SortDirection)
            .Skip(searchParam.PageIndex * searchParam.PageSize)
            .Take(searchParam.PageSize)
            .Select(j => new
            {
                j.Id,
                j.EmployeeId,
                j.ReferenceNo,
                EmployeeName = j.Employee.FullName,
                EmployeeCode = j.Employee.Code,
                OrganizationUnit = j.Employee.OrganizationUnit != null ? j.Employee.OrganizationUnit.Name : null,
                Company = j.Employee.Company != null ? j.Employee.Company.Name : null,
                Department = j.Employee.Department != null ? j.Employee.Department.Name : null,
                j.MonthlyCtc,
                j.OfferLetterDate,
                j.Employee.JoiningDate,
                j.ApprovalStatus,
                // Current designation: the open one, else the most recent.
                Designation = j.Employee.TblEmployeeDesignations
                    .OrderBy(d => d.EffectiveTo != null)
                    .ThenByDescending(d => d.EffectiveFrom)
                    .Select(d => new { d.DesignationId, d.Designation.Name })
                    .FirstOrDefault(),
                Qualification = j.Employee.TblEmployeeQualifications
                    .OrderByDescending(q => q.IsLatestQualification)
                    .ThenByDescending(q => q.YearOfPassing)
                    .Select(q => q.Specialization == null ? q.AcademicStream : q.AcademicStream + " - " + q.Specialization)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        // Salary grade hangs off the designation, so it is looked up for this page's designations only.
        var designationIds = rows.Where(r => r.Designation != null).Select(r => r.Designation!.DesignationId).Distinct().ToList();
        var grades = await context.TblDesignationGradeMappings
            .AsNoTracking()
            .Where(g => designationIds.Contains(g.DesignationId) && g.IsActive)
            .Select(g => new { g.DesignationId, g.Grade.Name })
            .ToListAsync(cancellationToken);
        var gradeByDesignation = grades
            .GroupBy(g => g.DesignationId)
            .ToDictionary(g => g.Key, g => g.First().Name);

        var results = rows.Select(r => new JoiningInfoApprovalListItem
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            ReferenceNo = r.ReferenceNo,
            EmployeeName = r.EmployeeName,
            EmployeeCode = r.EmployeeCode,
            Designation = r.Designation?.Name,
            OrganizationUnit = r.OrganizationUnit,
            Company = r.Company,
            Department = r.Department,
            SalaryGrade = r.Designation != null && gradeByDesignation.TryGetValue(r.Designation.DesignationId, out var grade) ? grade : null,
            MonthlyCtc = r.MonthlyCtc,
            OfferLetterDate = r.OfferLetterDate,
            JoiningDate = r.JoiningDate,
            Qualification = r.Qualification,
            ApprovalStatus = r.ApprovalStatus ?? JoiningInfoApprovalStatus.Pending
        }).ToList();

        return new GetJoiningInfoApprovalsResponse(results, totalRecords, true);
    }

    private static IQueryable<QubeFin.Persistence.Entities.TblJoiningInfo> ApplySort(IQueryable<QubeFin.Persistence.Entities.TblJoiningInfo> query, string sortOn, string sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return sortOn?.ToLowerInvariant() switch
        {
            "employeename" => descending ? query.OrderByDescending(j => j.Employee.FullName) : query.OrderBy(j => j.Employee.FullName),
            "organizationunit" => descending ? query.OrderByDescending(j => j.Employee.OrganizationUnit!.Name) : query.OrderBy(j => j.Employee.OrganizationUnit!.Name),
            "company" => descending ? query.OrderByDescending(j => j.Employee.Company!.Name) : query.OrderBy(j => j.Employee.Company!.Name),
            "department" => descending ? query.OrderByDescending(j => j.Employee.Department!.Name) : query.OrderBy(j => j.Employee.Department!.Name),
            "monthlyctc" => descending ? query.OrderByDescending(j => j.MonthlyCtc) : query.OrderBy(j => j.MonthlyCtc),
            "joiningdate" => descending ? query.OrderByDescending(j => j.Employee.JoiningDate) : query.OrderBy(j => j.Employee.JoiningDate),
            _ => descending ? query.OrderByDescending(j => j.CreatedOn) : query.OrderBy(j => j.CreatedOn)
        };
    }
}
#endregion
