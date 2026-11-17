using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeesBySearchQuery(EmployeeSearchParam SearchParam) : IRequest<GetEmployeesBySearchResponse>;
#endregion

#region --- RESPONSE ---
public record GetEmployeesBySearchResponse(IReadOnlyList<EmployeeSearchResult> Employees, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeesBySearchQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeesBySearchQuery, GetEmployeesBySearchResponse>
{
    public async Task<GetEmployeesBySearchResponse> Handle(GetEmployeesBySearchQuery request, CancellationToken cancellationToken)
    {
        var skipRecordCount = request.SearchParam.PageIndex * request.SearchParam.PageSize;
        var filterEntitiesQuery = context.TblEmployees.AsNoTracking().AsQueryable();

        if (request.SearchParam.CompanyId != null && request.SearchParam.CompanyId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.CompanyId == request.SearchParam.CompanyId);
        }
        if (request.SearchParam.SearchOrganizationUnitId != null && request.SearchParam.SearchOrganizationUnitId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnitId == request.SearchParam.SearchOrganizationUnitId);
        }
        if (!string.IsNullOrEmpty(request.SearchParam.SearchType))
        {
            filterEntitiesQuery = request.SearchParam.SearchType.Equals("C") ? filterEntitiesQuery.Where(m => m.IsActive) :
                request.SearchParam.SearchType.Equals("L") ? filterEntitiesQuery.Where(m => !m.IsActive) : filterEntitiesQuery;
        }
        if (!string.IsNullOrEmpty(request.SearchParam.SearchText))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.Code!.Contains(request.SearchParam.SearchText.Trim()) || m.FullName.Contains(request.SearchParam.SearchText.Trim())
                || m.MobileNo!.Contains(request.SearchParam.SearchText.Trim()) || m.PersonalEmail!.Contains(request.SearchParam.SearchText.Trim()) || m.OfficialEmail!.Contains(request.SearchParam.SearchText.Trim()));
        }
        if (request.SearchParam.SrchJoiningDate != null)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.JoiningDate == request.SearchParam.SrchJoiningDate);
        }

        if (request.SearchParam.SortOn is not null && request.SearchParam.SortDirection is not null)
        {
            filterEntitiesQuery = request.SearchParam.SortOn switch
            {
                "code" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Code) : filterEntitiesQuery.OrderBy(m => m.Code),
                "name" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.FirstName) : filterEntitiesQuery.OrderBy(m => m.FirstName),
                "joiningDt" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.JoiningDate) : filterEntitiesQuery.OrderBy(m => m.JoiningDate),
                _ => request.SearchParam.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.Code) : filterEntitiesQuery.OrderBy(m => m.Code),
            };
        }
        
        var totalCount = await filterEntitiesQuery.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var employees = await filterEntitiesQuery
    .Skip(skipRecordCount)
    .Take(request.SearchParam.PageSize)
    .Select(m => new
    {
        Employee = m,

        CurrentDesignation = m.TblEmployeeDesignations
            .OrderByDescending(ed => ed.EffectiveTo == null)
            .ThenByDescending(ed => ed.EffectiveFrom)
            .Select(ed => new
            {
                ed.DesignationId,
                DesignationName = ed.Designation.Name,

                SalaryGrade = ed.Designation.TblDesignationGradeMappings
                    .Where(g => g.IsActive)
                    .Select(g => g.Grade.Name)
                    .FirstOrDefault()
                    ??
                    ed.Designation.TblDesignationGradeMappings
                        .Select(g => g.Grade.Name)
                        .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new EmployeeSearchResult
    {
        Id = x.Employee.Id,
        Code = x.Employee.Code,
        FullName = x.Employee.FullName,

        CompanyName = x.Employee.CompanyId != null
            ? x.Employee.Company.EmpCodeAlias
            : null,

        OrganizationUnitName = x.Employee.OrganizationUnitId != null
            ? x.Employee.OrganizationUnit.Name
            : null,

        Email = x.Employee.OfficialEmail,
        Mobile = x.Employee.MobileNo,
        Gender = x.Employee.Gender,
        JoiningDate = x.Employee.JoiningDate,
        SeperationDate = x.Employee.SeparationDate,
        IsActive = x.Employee.IsActive,


        Designation = x.CurrentDesignation != null
            ? x.CurrentDesignation.DesignationName
            : null,

        Grade = x.CurrentDesignation != null
            ? x.CurrentDesignation.SalaryGrade
            : null
    })
    .ToListAsync(cancellationToken: cancellationToken)
    .ConfigureAwait(false);
        return new GetEmployeesBySearchResponse(employees, totalCount);
    }
}
#endregion

