using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IBcListRepository
{
    Task<List<BcGroupListResult>> GetGroupsAsync(BcListFilter filter, CancellationToken cancellationToken = default);
    Task<List<BcMemberListResult>> GetMembersAsync(BcListFilter filter, CancellationToken cancellationToken = default);
}

// Paged group and member lists through Global.USP_GetBCGroupList and Global.USP_GetBCMemberList
public class BcListRepository(QubeFinDataContext context) : IBcListRepository
{
    public async Task<List<BcGroupListResult>> GetGroupsAsync(BcListFilter filter, CancellationToken cancellationToken = default) =>
        await context.Set<BcGroupListResult>()
            .FromSqlRaw(
                "EXEC [Global].[USP_GetBCGroupList] @UserId, @OrganisationUnitId, @CoEmployeeId, @SearchText, @SortOn, @SortDirection, @PageIndex, @PageSize",
                Value("@UserId", filter.UserId),
                Value("@OrganisationUnitId", filter.OrganisationUnitId),
                Value("@CoEmployeeId", filter.CoEmployeeId),
                Text("@SearchText", filter.SearchText),
                Text("@SortOn", filter.SortOn),
                Text("@SortDirection", filter.SortDirection),
                Value("@PageIndex", filter.PageIndex),
                Value("@PageSize", filter.PageSize))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<List<BcMemberListResult>> GetMembersAsync(BcListFilter filter, CancellationToken cancellationToken = default) =>
        await context.Set<BcMemberListResult>()
            .FromSqlRaw(
                "EXEC [Global].[USP_GetBCMemberList] @UserId, @OrganisationUnitId, @CoEmployeeId, @GroupId, @SearchText, @SortOn, @SortDirection, @PageIndex, @PageSize",
                Value("@UserId", filter.UserId),
                Value("@OrganisationUnitId", filter.OrganisationUnitId),
                Value("@CoEmployeeId", filter.CoEmployeeId),
                Value("@GroupId", filter.GroupId),
                Text("@SearchText", filter.SearchText),
                Text("@SortOn", filter.SortOn),
                Text("@SortDirection", filter.SortDirection),
                Value("@PageIndex", filter.PageIndex),
                Value("@PageSize", filter.PageSize))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    private static SqlParameter Value(string name, object? value) => new(name, value ?? DBNull.Value);

    private static SqlParameter Text(string name, string? value) =>
        new(name, SqlDbType.NVarChar, 100) { Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim() };
}
