using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Collection;

namespace QubeFin.Collection.Persistence.Repositories
{
    public interface ICollectionRepository
    {
        Task<List<CreditOfficer>> GetCreditOfficersAsync(Guid employeeId, CancellationToken cancellationToken = default);
        Task<List<CreditOfficerGroup>> GetCreditOfficerGroupsAsync(Guid employeeId, Guid designationId, CancellationToken cancellationToken = default);
        Task<List<BcGroupLoan>> GetGroupLoansAsync(Guid employeeId, Guid designationId, Guid? bcGroupId, DateOnly collectionDate, BcGroupLoanSearchParam search, CancellationToken cancellationToken = default);
    }

    public class CollectionRepository(QubeFinDataContext context) : ICollectionRepository
    {
        private const string CreditOfficerPost = "Credit Officer";
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 200;
        private const char LikeEscapeChar = '!';

        // The logged-in user's reach: their own organization unit (from the running designation, else the employee's
        // own unit) plus every unit below it - HO -> Division -> Area -> Branch.
        //
        // The subtree is walked by a recursive CTE inside SQL Server instead of pulling Global.Tbl_OrganizationUnit
        // into memory and sending the ids back as an IN list, which an HO user would both drag across the wire and
        // grow past the 2100 parameter limit as branches are added.
        private const string LoginUnitTreeCte = """
            WITH LoginUnit AS (
                SELECT TOP (1) u.OrganizationUnitId
                FROM (
                    SELECT d.OrganizationUnitId, 1 AS Priority, ed.EffectiveFrom
                    FROM Hrms.Tbl_EmployeeDesignation ed
                    INNER JOIN Hrms.Tbl_Designation d ON d.Id = ed.DesignationId
                    WHERE ed.EmployeeId = @EmployeeId
                      AND ed.EffectiveTo IS NULL
                    UNION ALL
                    SELECT e.OrganizationUnitId, 2 AS Priority, NULL AS EffectiveFrom
                    FROM Hrms.Tbl_Employee e
                    WHERE e.Id = @EmployeeId
                      AND e.OrganizationUnitId IS NOT NULL
                ) u
                ORDER BY u.Priority, u.EffectiveFrom DESC
            ),
            UnitTree AS (
                SELECT ou.Id
                FROM Global.Tbl_OrganizationUnit ou
                INNER JOIN LoginUnit lu ON lu.OrganizationUnitId = ou.Id
                UNION ALL
                SELECT child.Id
                FROM Global.Tbl_OrganizationUnit child
                INNER JOIN UnitTree parent ON parent.Id = child.ParentId
            )
            """;

        // Active 'Credit Officer' designations currently held by an active employee, inside the login user's reach.
        // Index support: IX_Tbl_EmployeeDesignation_EmployeeId_EffectiveTo, IX_Tbl_EmployeeDesignation_DesignationId_EffectiveTo.
        public async Task<List<CreditOfficer>> GetCreditOfficersAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            var employeeIdParameter = new SqlParameter("@EmployeeId", employeeId);
            var postNameParameter = new SqlParameter("@PostName", CreditOfficerPost);

            var sql = $"""
                {LoginUnitTreeCte}
                SELECT
                    ed.EmployeeId AS EmployeeId,
                    e.FullName AS EmployeeName,
                    e.Code AS EmployeeCode,
                    ed.DesignationId AS DesignationId,
                    d.Name AS DesignationName,
                    d.IsIncharge AS IsIncharge,
                    d.OrganizationUnitId AS OrganizationUnitId,
                    ou.Name AS OrganizationUnitName,
                    ou.CodeVal AS OrganizationCode,
                    out2.Name AS OrganizationUnitTypeName,
                    ed.EffectiveFrom AS EffectiveFrom
                FROM Hrms.Tbl_EmployeeDesignation ed
                INNER JOIN Hrms.Tbl_Designation d ON d.Id = ed.DesignationId
                INNER JOIN UnitTree t ON t.Id = d.OrganizationUnitId
                INNER JOIN Hrms.Tbl_Post p ON p.Id = d.PostId
                INNER JOIN Hrms.Tbl_Employee e ON e.Id = ed.EmployeeId
                INNER JOIN Global.Tbl_OrganizationUnit ou ON ou.Id = d.OrganizationUnitId
                INNER JOIN Global.Tbl_OrganizationUnitType out2 ON out2.Id = ou.OrganizationUnitTypeId
                WHERE ed.EffectiveTo IS NULL
                  AND d.IsActive = 1
                  AND e.IsActive = 1
                  AND p.Name = @PostName
                ORDER BY ou.Name, e.FullName
                OPTION (MAXRECURSION 100)
                """;

            return await context.Database
                .SqlQueryRaw<CreditOfficer>(sql, employeeIdParameter, postNameParameter)
                .ToListAsync(cancellationToken);
        }

        // BC groups tagged to one credit officer's designation (BC.Tbl_BCGroup.DesignationId), restricted to the login
        // user's reach so a designation id from outside their units returns nothing. Index support:
        // IX_Tbl_BCGroup_OrganizationUnitId_DesignationId. The officer's name comes from an OUTER APPLY TOP (1) rather
        // than a join, so a designation held by two employees at once cannot multiply the group rows.
        public async Task<List<CreditOfficerGroup>> GetCreditOfficerGroupsAsync(Guid employeeId, Guid designationId, CancellationToken cancellationToken = default)
        {
            var employeeIdParameter = new SqlParameter("@EmployeeId", employeeId);
            var designationIdParameter = new SqlParameter("@DesignationId", designationId);

            var sql = $"""
                {LoginUnitTreeCte}
                SELECT
                    g.Id AS Id,
                    g.Code AS Code,
                    g.Name AS Name,
                    g.DesignationId AS DesignationId,
                    d.Name AS DesignationName,
                    co.EmployeeId AS CreditOfficerId,
                    co.FullName AS CreditOfficerName,
                    g.OrganizationUnitId AS OrganizationUnitId,
                    ou.Name AS OrganizationUnitName,
                    ou.CodeVal AS OrganizationCode,
                    g.FormationDate AS FormationDate,
                    g.CollectionDay AS CollectionDay,
                    g.CollectionTime AS CollectionTime,
                    g.IsMonthlyGroup AS IsMonthlyGroup,
                    g.BiWeeklyStartDate AS BiWeeklyStartDate,
                    g.ContactNo AS ContactNo,
                    g.AddressLine1 AS AddressLine1,
                    g.PinCode AS PinCode
                FROM BC.Tbl_BCGroup g
                INNER JOIN UnitTree t ON t.Id = g.OrganizationUnitId
                INNER JOIN Hrms.Tbl_Designation d ON d.Id = g.DesignationId
                INNER JOIN Global.Tbl_OrganizationUnit ou ON ou.Id = g.OrganizationUnitId
                OUTER APPLY (
                    SELECT TOP (1) e.Id AS EmployeeId, e.FullName
                    FROM Hrms.Tbl_EmployeeDesignation ed
                    INNER JOIN Hrms.Tbl_Employee e ON e.Id = ed.EmployeeId
                    WHERE ed.DesignationId = g.DesignationId
                      AND ed.EffectiveTo IS NULL
                      AND e.IsActive = 1
                    ORDER BY ed.EffectiveFrom DESC
                ) co
                WHERE g.DesignationId = @DesignationId
                ORDER BY g.Name
                OPTION (MAXRECURSION 100)
                """;

            return await context.Database
                .SqlQueryRaw<CreditOfficerGroup>(sql, employeeIdParameter, designationIdParameter)
                .ToListAsync(cancellationToken);
        }

        // Group wise BC loan search for one collection date: loans of the members of the BC groups tagged to a credit
        // officer's designation, optionally narrowed to one of those groups, restricted to the login user's reach so a
        // designation or group id from outside their units returns nothing. Chain is
        // BC.Tbl_BCLoan -> BC.Tbl_BCMember -> BC.Tbl_BCGroup, indexed by IX_Tbl_BCMember_BCGroupId. Search, sort and
        // paging all resolve in this one statement: COUNT(*) OVER() carries the pre-paging total, so a page and its row
        // count cost one round trip, and OFFSET/FETCH leaves the unread rows in SQL Server instead of paging in memory.
        public async Task<List<BcGroupLoan>> GetGroupLoansAsync(Guid employeeId, Guid designationId, Guid? bcGroupId, DateOnly collectionDate, BcGroupLoanSearchParam search, CancellationToken cancellationToken = default)
        {
            var pageSize = search.PageSize <= 0 ? DefaultPageSize : Math.Min(search.PageSize, MaxPageSize);
            var pageIndex = Math.Max(search.PageIndex, 0);

            var employeeIdParameter = new SqlParameter("@EmployeeId", employeeId);
            var designationIdParameter = new SqlParameter("@DesignationId", designationId);
            var bcGroupIdParameter = new SqlParameter("@BcGroupId", SqlDbType.UniqueIdentifier)
            {
                Value = bcGroupId.HasValue ? bcGroupId.Value : DBNull.Value
            };
            var collectionDateParameter = new SqlParameter("@CollectionDate", SqlDbType.Date)
            {
                Value = collectionDate.ToDateTime(TimeOnly.MinValue)
            };
            var searchTextParameter = new SqlParameter("@SearchText", SqlDbType.NVarChar, 200)
            {
                Value = string.IsNullOrWhiteSpace(search.SearchText)
                    ? DBNull.Value
                    : $"%{EscapeLikePattern(search.SearchText.Trim())}%"
            };
            var isClosedParameter = new SqlParameter("@IsClosed", SqlDbType.Bit)
            {
                Value = search.IsClosed.HasValue ? search.IsClosed.Value : DBNull.Value
            };
            var skipParameter = new SqlParameter("@Skip", pageIndex * pageSize);
            var pageSizeParameter = new SqlParameter("@PageSize", pageSize);

            // A whitelisted column and direction, never the caller's text, so the ORDER BY cannot be injected.
            var orderBy = ResolveLoanSort(search.SortOn, search.SortDirection);

            var sql = $"""
                {LoginUnitTreeCte}
                SELECT
                    l.Id AS Id,
                    l.LoanNo AS LoanNo,
                    g.Id AS BcGroupId,
                    g.Code AS GroupCode,
                    g.Name AS GroupName,
                    m.Id AS BcMemberId,
                    m.Code AS MemberCode,
                    LTRIM(RTRIM(CONCAT(m.FirstName, ' ' + m.MiddleName, ' ', m.LastName))) AS MemberName,
                    m.MobileNo AS MemberMobileNo,
                    l.LoanProductName AS LoanProductName,
                    l.LoanPurpose AS LoanPurpose,
                    l.LoanAmount AS LoanAmount,
                    l.RateOfInterest AS RateOfInterest,
                    l.LoanTenure AS LoanTenure,
                    l.EMIAmount AS EmiAmount,
                    l.DisburseDate AS DisburseDate,
                    l.DisburseAmount AS DisburseAmount,
                    l.TotalPaidAmount AS TotalPaidAmount,
                    l.TotalPaidInstallment AS TotalPaidInstallment,
                    l.CurrentOutstadingAmount AS CurrentOutstandingAmount,
                    l.OverdueAmount AS OverdueAmount,
                    l.DPDDays AS DpdDays,
                    l.IsClosed AS IsClosed,
                    l.ClosedDate AS ClosedDate,
                    c.Id AS CollectionId,
                    c.EmiAmount AS CollectionEmiAmount,
                    c.CollectionAmount AS CollectedAmount,
                    ISNULL(c.IsDemandDate, 0) AS IsDemandDate,
                    ISNULL(c.IsCollected, 0) AS IsCollected,
                    c.Remarks AS CollectionRemarks,
                    COUNT(*) OVER() AS TotalRecords
                FROM BC.Tbl_BCLoan l
                INNER JOIN BC.Tbl_BCMember m ON m.Id = l.BCMemberId
                INNER JOIN BC.Tbl_BCGroup g ON g.Id = m.BCGroupId
                INNER JOIN UnitTree t ON t.Id = g.OrganizationUnitId
                OUTER APPLY (
                    SELECT TOP (1) lc.Id, lc.EmiAmount, lc.CollectionAmount, lc.IsDemandDate, lc.IsCollected, lc.Remarks
                    FROM BC.Tbl_BCLoanCollection lc
                    WHERE lc.LoanId = l.Id
                      AND lc.CollectionDate = @CollectionDate
                    ORDER BY lc.IsDemandDate DESC, lc.CreatedOn DESC
                ) c
                WHERE g.DesignationId = @DesignationId
                  AND (@BcGroupId IS NULL OR m.BCGroupId = @BcGroupId)
                  AND (@IsClosed IS NULL OR l.IsClosed = @IsClosed)
                  AND (@SearchText IS NULL
                       OR l.LoanNo LIKE @SearchText ESCAPE '!'
                       OR m.Code LIKE @SearchText ESCAPE '!'
                       OR m.FirstName LIKE @SearchText ESCAPE '!'
                       OR m.LastName LIKE @SearchText ESCAPE '!'
                       OR m.MobileNo LIKE @SearchText ESCAPE '!')
                ORDER BY {orderBy}, l.Id
                OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY
                OPTION (MAXRECURSION 100)
                """;

            return await context.Database
                .SqlQueryRaw<BcGroupLoan>(
                    sql,
                    employeeIdParameter,
                    designationIdParameter,
                    bcGroupIdParameter,
                    collectionDateParameter,
                    searchTextParameter,
                    isClosedParameter,
                    skipParameter,
                    pageSizeParameter)
                .ToListAsync(cancellationToken);
        }

        private static string ResolveLoanSort(string? sortOn, string? sortDirection)
        {
            string[] columns = (sortOn ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "loanno" => ["l.LoanNo"],
                "groupname" => ["g.Name"],
                "membername" => ["m.FirstName", "m.LastName"],
                "membercode" => ["m.Code"],
                "loanproductname" => ["l.LoanProductName"],
                "loanamount" => ["l.LoanAmount"],
                "disbursedate" => ["l.DisburseDate"],
                "currentoutstandingamount" => ["l.CurrentOutstadingAmount"],
                "overdueamount" => ["l.OverdueAmount"],
                "dpddays" => ["l.DPDDays"],
                // The default keeps a group's loans together, since the search now spans every group of the officer.
                _ => ["g.Name", "l.LoanNo"]
            };

            var direction = string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            return string.Join(", ", columns.Select(column => $"{column} {direction}"));
        }

        // Keeps a wildcard typed by the user from widening the search.
        private static string EscapeLikePattern(string value) =>
            value
                .Replace(LikeEscapeChar.ToString(), $"{LikeEscapeChar}{LikeEscapeChar}")
                .Replace("%", $"{LikeEscapeChar}%")
                .Replace("_", $"{LikeEscapeChar}_")
                .Replace("[", $"{LikeEscapeChar}[");
    }
}
