using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface ILeaveRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task<string> AddAsync(Guid Id, Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, string address, string reason, string enclosedFileName, string enclosedFileNo);
    Task<bool> SubmitAsync(Guid id, Guid userId);
    Task<bool> CancelAsync(Guid id, string? reason, Guid userId);
    Task<List<GetAllPendingFitnessApprovalResposne>> GetPendingFitnessApprovalList(CancellationToken cancellationToken);
}

public class LeaveRepository(QubeFinDataContext context) : ILeaveRepository
{
    public async Task<string> AddAsync(Guid Id, Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, string address, string reason, string enclosedFileName, string enclosedFileNo)
    {
        var returnMessage = @"";
        var parameters = new[]
        {
            new SqlParameter("@p_Id", Id),
            new SqlParameter("@p_EmployeeId", employeeId),
            new SqlParameter("@p_LeaveTypeId", leaveTypeId),
            new SqlParameter("@p_FromDate", fromDate),
            new SqlParameter("@p_ToDate", toDate),
            new SqlParameter("@p_Address", (object?)address ?? DBNull.Value),
            new SqlParameter("@p_Reason", (object?)reason ?? DBNull.Value),
            new SqlParameter("@p_FileName", (object?)enclosedFileName ?? DBNull.Value),
            new SqlParameter("@p_FileNo", (object?)enclosedFileNo ?? DBNull.Value),
        };
        try
        {
            var result = await context.Database.ExecuteSqlRawAsync(@"EXEC [Hrms].[USP_LeaveRequestSave]
              @p_Id,
              @p_EmployeeId,
              @p_LeaveTypeId,
              @p_FromDate,
              @p_ToDate,
              @p_Address,
              @p_Reason,
              @p_FileName,
              @p_FileNo", parameters);
        }
        catch (SqlException sqlEx)
        {
            // swallow
            returnMessage = sqlEx.Errors[0].Message;
        }
        catch (Exception ex)
        {
            // swallow
            returnMessage = ex.Message;
        }

        return returnMessage;
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid id)
    {
        var leaveRequestEntity = await context.TblLeaveRequests.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        return leaveRequestEntity?.ToDomain();

    }

    public async Task<bool> SubmitAsync(Guid id, Guid userId)
    {
        var leaveRequestEntity = await context.TblLeaveRequests.SingleOrDefaultAsync(x => x.Id == id)?? throw new Exception("Leave Request not found for the given Id");

        if (leaveRequestEntity.IsSubmitted)
        {
            throw new Exception("Leave Request has already been submitted.");
        }

        var approvalEvent = await context.TblApprovalRequestEvents.AsNoTracking().SingleOrDefaultAsync(x =>x.MappingId == id && x.Category == "LEAVE" && !x.IsSubmitted);

        if (approvalEvent == null)
        {
            throw new Exception("Approver or recommender has not been configured for this leave request.");
        }

        if (approvalEvent.ReceiverDesignationId == null)
        {
            throw new Exception("Receiver designation has not been configured for this leave request.");
        }

        var employeeName = await context.TblEmployees.Where(x => x.Id == leaveRequestEntity.EmployeeId).Select(x => x.FullName).SingleOrDefaultAsync();

        leaveRequestEntity.IsSubmitted = true;
        leaveRequestEntity.SubmittedOn = DateTime.Now;
        leaveRequestEntity.SubmittedBy = userId;
        leaveRequestEntity.CurrentStatus = "Pending";

        var notification = new TblNotification
        {
            Id = Guid.NewGuid(),
            DesignationId = approvalEvent.ReceiverDesignationId,
            Title = "Leave Request",
            Icon = "triangle-alert",
            Message = $"{employeeName}'s leave request has been forwarded for your approval.",
            NotificationType = "info",
            ReferenceId = id,
            ReferenceType = "LEAVE",
            ActionUrl = "/secure/hrms/leave-approvals",
            IsRead = false,
            CreatedBy = userId,
            CreatedOn = DateTime.Now
        };

        await context.TblNotifications.AddAsync(notification);

        return true;
    }

    public async Task<bool> CancelAsync(Guid id, string? reason, Guid userId)
    {
        var leaveRequestEntity = await context.TblLeaveRequests.SingleOrDefaultAsync(m => m.Id == id) ?? throw new Exception("Leave Request not found for the given Id");
        leaveRequestEntity.CurrentStatus = "Cancelled";
        leaveRequestEntity.RejectedReason = reason;

        return true;
    }

    public async Task<List<GetAllPendingFitnessApprovalResposne>> GetPendingFitnessApprovalList(CancellationToken cancellationToken)
    {

        var leaveTypes = new[] { "ML", "MML" };
        var leaveEntities = await context.TblLeaveRequests.Include(m => m.Employee).Include(m => m.LeaveType).AsNoTracking().Where(m => m.CurrentStatus.Trim() == "Approved" && leaveTypes.Contains(m.LeaveType.Alias) && !m.IsFitnessReportApproved && !string.IsNullOrWhiteSpace(m.FitnessReportAttachment)).ToListAsync(cancellationToken);
        return leaveEntities.Select(m => new GetAllPendingFitnessApprovalResposne
        {
            EmployeeName = m.Employee.FullName + "( " + m.Employee.Code + " )",
            LeaveRequestId = m.Id,
            LeaveType = m.LeaveType.Title + "( " + m.LeaveType.Alias + " )",
            FromDate = m.FromDate, 
            EndDate = m.ToDate,
            TotalDays = m.TotalDays != null ? m.TotalDays.Value : 0,
        }).ToList();
    }
}
