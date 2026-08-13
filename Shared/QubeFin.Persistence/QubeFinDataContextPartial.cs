using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Persistence;

public partial class QubeFinDataContext : DbContext, IUnitOfWork
{
    public virtual DbSet<EmployeeLeaveRequest> EmployeeLeaveRequest { get; set; }
    public virtual DbSet<ApprovalWorkflowEventGroupItem> ApprovalWorkflowEventGroupItem { get; set; }
    public virtual DbSet<Payslip> Payslips { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuHierarchyItem>().HasNoKey().ToView(null);
        modelBuilder.Entity<AdministrativeHierarchyItem>().HasNoKey().ToView(null);
        modelBuilder.Entity<OrganizationUnitHierarchyItem>().HasNoKey().ToView(null);
        modelBuilder.Entity<SurveyResults>().HasNoKey().ToView(null);
        modelBuilder.Entity<EmployeewiseLeaveTypeBalance>().HasNoKey().ToView(null);
        modelBuilder.Entity<EmployeeLeaveRequest>().HasNoKey().ToView(null);
        modelBuilder.Entity<RegularizationSearchResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<RegularizationResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<RegularizationApprovalSearchResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<LeaveRequestResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<LeaveApprovalSearchResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<ApprovalWorkflowEventGroupItem>().HasNoKey().ToView(null);
        modelBuilder.Entity<LeavePrayerResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<LeavePrayerApprovalSearchResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<Payslip>().HasNoKey().ToView(null);
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
    public async Task<List<ApprovalWorkflowEventGroupItem>> SP_GetApprovalWorkflowEventGroupItems()
    {
        return await ApprovalWorkflowEventGroupItem
            .FromSqlInterpolated($"[Hrms].[USP_GetApprovalWorkflowEvents]")
            .ToListAsync();
    }
    public async Task<List<Payslip>> SP_GetEmployeePayslip(Guid employeeId)
    {
        return await Payslips
            .FromSqlInterpolated($"[Payroll].[USP_GetEmployeePayslip] @EmployeeId = {employeeId}")
            .ToListAsync();
    }
}