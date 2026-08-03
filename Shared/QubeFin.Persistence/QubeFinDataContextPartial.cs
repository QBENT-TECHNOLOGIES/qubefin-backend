using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Persistence;

public partial class QubeFinDataContext : DbContext, IUnitOfWork
{
    public virtual DbSet<EmployeeLeaveRequest> EmployeeLeaveRequest { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuHierarchyItem>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<AdministrativeHierarchyItem>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<OrganizationUnitHierarchyItem>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<SurveyResults>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<EmployeewiseLeaveTypeBalance>()
            .HasNoKey()
            .ToView(null);

        modelBuilder.Entity<EmployeeLeaveRequest>()
            .HasNoKey()
            .ToView(null);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<EmployeeLeaveRequest>> SP_GetEmployeeLeaveRequests(int year, Guid employeeId)
    {
        return await EmployeeLeaveRequest
            .FromSqlInterpolated($"[Hrms].[USP_GetLeaveRequestsByEmployee] @p_Year = {year}, @p_EmployeeId = {employeeId}")
            .ToListAsync();
    }
}