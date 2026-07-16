using MediatR;
using QubeFin.Persistence;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.SurveyCommittees.Models;

namespace QubeFin.Global.Application.SurveyCommittees.Queries;

#region --- QUERY ---
public record FilterCommitteeMemberQuery(string? SearchText, string? SortOn, string? SortDirection, int PageIndex, int PageSize) : IRequest<FilterCommitteeMemberResponse>;
#endregion

#region --- RESPONSE ---
public record FilterCommitteeMemberResponse(IReadOnlyList<SurveyCommitteeMemberResponse> SurveyCommittees, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class FilterCommitteeMemberQueryHandler(QubeFinDataContext context) : IRequestHandler<FilterCommitteeMemberQuery, FilterCommitteeMemberResponse>
{
    public async Task<FilterCommitteeMemberResponse> Handle(FilterCommitteeMemberQuery request, CancellationToken cancellationToken)
    {
        var skipRecordCount = request.PageIndex * request.PageSize;
        var filterEntitiesQuery = context.TblSurveyCommittees.Include(m => m.Employee).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim().ToLower();

            filterEntitiesQuery = filterEntitiesQuery.Where(x => x.Employee.FullName.ToLower().Contains(search));
        }

        if (request.SortOn is not null && request.SortDirection is not null)
        {
            filterEntitiesQuery = request.SortOn switch
            {
                "employeeName" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Employee.FullName) : filterEntitiesQuery.OrderBy(m => m.Employee.FullName),
                "isLead" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.IsLead) : filterEntitiesQuery.OrderBy(m => m.IsLead),
                "isActive" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.IsActive) : filterEntitiesQuery.OrderBy(m => m.IsActive),
                "assignedFrom" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.AssignedFrom) : filterEntitiesQuery.OrderBy(m => m.AssignedFrom),
                "assignedTo" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.AssignedTo) : filterEntitiesQuery.OrderBy(m => m.AssignedTo),
                _ => request.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.Employee.FullName) : filterEntitiesQuery.OrderBy(m => m.Employee.FullName),
            };
        }

        var totalCount = await filterEntitiesQuery.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var committeeMembers = await filterEntitiesQuery.Skip(skipRecordCount).Take(request.PageSize)
            .Select(m => new SurveyCommitteeMemberResponse
            {
                Id = m.Id,
                EmployeeId = m.Employee.Id,
                EmployeeName = m.Employee.FullName,
                IsLead = m.IsLead,
                IsActive = m.IsActive,
                AssignedFrom = m.AssignedFrom,
                AssignedTo = m.AssignedTo
            })
            .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        return new FilterCommitteeMemberResponse(committeeMembers, totalCount);
    }
}
#endregion  