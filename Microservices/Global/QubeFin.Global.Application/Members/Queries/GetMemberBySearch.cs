using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.Members.Models;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Members.Queries;

#region --- QUERY ---
public record GetMembersBySearchQuery(MemberSearchParam SearchParam) : IRequest<GetMembersBySearchResponse>;
#endregion

#region --- RESPONSE ---
public record GetMembersBySearchResponse(IReadOnlyList<MemberSearchResult> Members, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetMembersBySearchQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetMembersBySearchQuery, GetMembersBySearchResponse>
{
    public async Task<GetMembersBySearchResponse> Handle(GetMembersBySearchQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.SearchParam.PageSize > 0 ? request.SearchParam.PageSize : 10;
        var skipRecordCount = request.SearchParam.PageIndex * pageSize;

        var filterEntitiesQuery = context.TblMembers.AsNoTracking().AsQueryable();

        if (request.SearchParam.BranchCode is > 0)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.BranchCode == request.SearchParam.BranchCode.Value);
        }

        if (request.SearchParam.GroupId is not null && request.SearchParam.GroupId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.GroupId == request.SearchParam.GroupId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchParam.SearchText))
        {
            var searchText = request.SearchParam.SearchText.Trim();
            filterEntitiesQuery = filterEntitiesQuery.Where(m =>
                (m.Code != null && m.Code.Contains(searchText)) ||
                m.FullName.Contains(searchText));
        }

        var isDesc = request.SearchParam.SortDirection?.Equals("DESC", StringComparison.OrdinalIgnoreCase) == true;

        filterEntitiesQuery = request.SearchParam.SortOn?.ToLowerInvariant() switch
        {
            "code" => isDesc ? filterEntitiesQuery.OrderByDescending(m => m.Code) : filterEntitiesQuery.OrderBy(m => m.Code),
            "name" or "fullname" => isDesc ? filterEntitiesQuery.OrderByDescending(m => m.FirstName) : filterEntitiesQuery.OrderBy(m => m.FirstName),
            "branchcode" => isDesc ? filterEntitiesQuery.OrderByDescending(m => m.BranchCode) : filterEntitiesQuery.OrderBy(m => m.BranchCode),
            "mobileno" => isDesc ? filterEntitiesQuery.OrderByDescending(m => m.MobileNo) : filterEntitiesQuery.OrderBy(m => m.MobileNo),
            _ => isDesc ? filterEntitiesQuery.OrderByDescending(m => m.CreatedOn) : filterEntitiesQuery.OrderBy(m => m.CreatedOn),
        };

        var totalCount = await filterEntitiesQuery
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        var members = await filterEntitiesQuery
            .Skip(skipRecordCount)
            .Take(pageSize)
            .Select(m => new MemberSearchResult
            {
                Id = m.Id,
                BranchCode = m.BranchCode,
                Code = m.Code,
                FullName = m.FullName,
                GuardianName = m.GuardianName,
                DateOfBirth = m.DateOfBirth,
                Gender = m.Gender,
                MobileNo = m.MobileNo,
                CreatedOn = m.CreatedOn
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new GetMembersBySearchResponse(members, totalCount);
    }
}
#endregion