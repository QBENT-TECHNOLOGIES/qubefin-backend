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
        var filterEntitiesQuery = context.TblEmployees.Include(e => e.Company).Include(e => e.OrganizationUnit).AsNoTracking().AsQueryable();

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
        var employees = await filterEntitiesQuery.Skip(skipRecordCount).Take(request.SearchParam.PageSize)
            .Select(m => new EmployeeSearchResult
            {
                Id = m.Id,
                Code = m.Code,
                FullName = m.FullName,
                CompanyName = m.CompanyId != null ? m.Company.Name : null,
                OrganizationUnitName = m.OrganizationUnitId != null ? m.OrganizationUnit.Name : null,
                Email = m.OfficialEmail,
                Mobile = m.MobileNo,
                Gender = m.Gender,
                JoiningDate = m.JoiningDate,
                SeperationDate = m.SeparationDate,
                IsActive = m.IsActive
            }).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return new GetEmployeesBySearchResponse(employees, totalCount);
    }
}
#endregion

