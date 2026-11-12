using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Persistence;

public partial class QubeFinDataContext : DbContext, IUnitOfWork
{
    public virtual DbSet<EmployeeLeaveRequest> EmployeeLeaveRequest { get; set; }
    public virtual DbSet<ApprovalWorkflowEventGroupItem> ApprovalWorkflowEventGroupItem { get; set; }
    public virtual DbSet<Payslip> Payslips { get; set; }
    public virtual DbSet<UserLoginInfoResult> UserLoginInfoResults { get; set; }

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
        modelBuilder.Entity<UserLoginInfoResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<LeaveTypeWiseBalanceResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<LastWorkingDateResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<EmployeeMonthlyCalendarResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<EmployeeLeaveMonthlyCalendarResponse>().HasNoKey().ToView(null);
        modelBuilder.Entity<GetInterviewCandidateDetail>().HasNoKey().ToView(null);
        modelBuilder.Entity<CandidateListResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<InterviewerCandidateListResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<BcGroupListResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<BcMemberListResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<UserWiseBranchItem>().HasNoKey().ToView(null);
        modelBuilder.Entity<BranchCreditOfficerItem>().HasNoKey().ToView(null);
        modelBuilder.Entity<FieldOfficerResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<JobSearchRowResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<TrackingViewerResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<TrackingOfficerResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<TrackingBranchResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<BranchCodeResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<JobDetailResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<JobTaskResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(e => e.TargetLocation).HasColumnType("geography");
            entity.Property(e => e.ArriveLocation).HasColumnType("geography");
            entity.Property(e => e.CompletedLocation).HasColumnType("geography");
        });
        modelBuilder.Entity<JobHistoryResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<JobTaskPointResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(e => e.TargetLocation).HasColumnType("geography");
        });
        modelBuilder.Entity<PendingPlaceTaskResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<GeoTagPointResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(e => e.Location).HasColumnType("geography");
        });
        modelBuilder.Entity<TrackingPlaceResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(e => e.Location).HasColumnType("geography");
        });
        modelBuilder.Entity<GeoTagResult>(entity =>
        {
            entity.HasNoKey().ToView(null);
            entity.Property(e => e.Location).HasColumnType("geography");
        });
        modelBuilder.Entity<GeoTagTargetResult>().HasNoKey().ToView(null);
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
    public async Task<List<UserLoginInfoResult>> SP_GetUserLoginInfo(Guid userId, Guid employeeId, string? deviceId)
    {
        return await UserLoginInfoResults
            .FromSqlInterpolated($"[Auth].[USP_GetUserLoginInfo] @p_UserId = {userId}, @p_EmployeeId = {employeeId}, @p_DeviceId = {deviceId}")
            .AsNoTracking()
            .ToListAsync();
    }
}
