using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeesBySearchQuery(string SearchType, string? SearchText, Guid? SearchOrganizationUnitId,
    string? SortOn, string? SortDirection, int PageIndex, int PageSize) : IRequest<GetEmployeesBySearchResponse>;
#endregion

#region --- RESPONSE ---
public record EmployeesBySearchResult(Guid Id, string? Code, string Name, string Office, string? Email, string? Mobile, DateOnly? JoiningDate, DateOnly? SeperationDate, bool IsActive);
public record GetEmployeesBySearchResponse(IReadOnlyList<EmployeesBySearchResult> Employees, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeesBySearchQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeesBySearchQuery, GetEmployeesBySearchResponse>
{
    public async Task<GetEmployeesBySearchResponse> Handle(GetEmployeesBySearchQuery request, CancellationToken cancellationToken)
    {
        var skipRecordCount = request.PageIndex * request.PageSize;
        var filterEntitiesQuery = context.TblEmployees.AsNoTracking().AsQueryable();

        if (request.SearchType.Equals("C"))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.IsActive);
        }
        if (request.SearchType.Equals("L"))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => !m.IsActive);
        }
        if (request.SearchOrganizationUnitId != null && request.SearchOrganizationUnitId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnitId == request.SearchOrganizationUnitId);
        }
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.Code!.Contains(request.SearchText.Trim()) || m.FullName.Contains(request.SearchText.Trim())
                || m.MobileNo!.Contains(request.SearchText.Trim()) || m.PersonalEmail!.Contains(request.SearchText.Trim()) || m.OfficialEmail!.Contains(request.SearchText.Trim()));
        }

        if (request.SortOn is not null && request.SortDirection is not null)
        {
            filterEntitiesQuery = request.SortOn switch
            {
                "code" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Code) : filterEntitiesQuery.OrderBy(m => m.Code),
                "name" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.FirstName) : filterEntitiesQuery.OrderBy(m => m.FirstName),
                "joiningDt" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.JoiningDate) : filterEntitiesQuery.OrderBy(m => m.JoiningDate),
                _ => request.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.Code) : filterEntitiesQuery.OrderBy(m => m.Code),
            };
        }

        var totalCount = await filterEntitiesQuery.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var employees = await filterEntitiesQuery.Skip(skipRecordCount).Take(request.PageSize)
            .Select(m => new EmployeesBySearchResult(m.Id, m.Code, string.Empty, string.Empty,
                m.OfficialEmail, m.MobileNo, m.JoiningDate, m.SeparationDate, m.IsActive))
            .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return new GetEmployeesBySearchResponse(employees, totalCount);
    }
}
#endregion

