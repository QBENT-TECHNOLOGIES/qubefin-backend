using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IOdMemberRepository
{
    Task<OdMemberSearchResult> SearchAsync(OdMemberFilter filter, DateOnly today, CancellationToken cancellationToken = default);
}

// OD Members page through Tracking.USP_SearchOdMembers
public class OdMemberRepository(QubeFinDataContext context) : IOdMemberRepository
{
    public async Task<OdMemberSearchResult> SearchAsync(OdMemberFilter filter, DateOnly today, CancellationToken cancellationToken = default)
    {
        var totalRecords = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var totalOverdue = new SqlParameter("@TotalOverdueAmount", SqlDbType.Decimal) { Precision = 18, Scale = 2, Direction = ParameterDirection.Output };

        var rows = await context.Set<OdMemberSearchRowResult>()
            .FromSqlRaw(
                "EXEC [Tracking].[USP_SearchOdMembers] @UserId, @Today, @SearchText, @OrganisationUnitId, @CreditOfficerId, @GroupId, " +
                "@DpdMin, @DpdMax, @OnlyUnassigned, @SortOn, @SortDescending, @PageIndex, @PageSize, @TotalRecords OUTPUT, @TotalOverdueAmount OUTPUT",
                new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = filter.UserId },
                new SqlParameter("@Today", SqlDbType.Date) { Value = today.ToDateTime(TimeOnly.MinValue) },
                new SqlParameter("@SearchText", SqlDbType.NVarChar, 200) { Value = string.IsNullOrWhiteSpace(filter.SearchText) ? DBNull.Value : filter.SearchText.Trim() },
                Id("@OrganisationUnitId", filter.OrganisationUnitId),
                Id("@CreditOfficerId", filter.CreditOfficerId),
                Id("@GroupId", filter.GroupId),
                new SqlParameter("@DpdMin", SqlDbType.Int) { Value = (object?)filter.DpdMin ?? DBNull.Value },
                new SqlParameter("@DpdMax", SqlDbType.Int) { Value = (object?)filter.DpdMax ?? DBNull.Value },
                new SqlParameter("@OnlyUnassigned", SqlDbType.Bit) { Value = filter.OnlyUnassigned },
                new SqlParameter("@SortOn", SqlDbType.NVarChar, 50) { Value = (object?)filter.SortOn ?? DBNull.Value },
                new SqlParameter("@SortDescending", SqlDbType.Bit) { Value = filter.SortDescending },
                new SqlParameter("@PageIndex", SqlDbType.Int) { Value = filter.PageIndex },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = filter.PageSize },
                totalRecords,
                totalOverdue)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Output values are read after the result set has been read to the end
        return new OdMemberSearchResult(
            rows,
            totalRecords.Value is int count ? count : 0,
            totalOverdue.Value is decimal amount ? amount : 0);
    }

    private static SqlParameter Id(string name, Guid? value) =>
        new(name, SqlDbType.UniqueIdentifier) { Value = (object?)value ?? DBNull.Value };
}
