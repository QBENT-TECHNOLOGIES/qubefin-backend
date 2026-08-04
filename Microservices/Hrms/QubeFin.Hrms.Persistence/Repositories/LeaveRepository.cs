using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface ILeaveRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task<string> AddAsync(Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, string address, string reason, string enclosedFileName, string enclosedFileNo);
    Task<bool> SubmitAsync(Guid id, Guid userId);
}

public class LeaveRepository(QubeFinDataContext context) : ILeaveRepository
{
    public async Task<string> AddAsync(Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, string address, string reason, string enclosedFileName, string enclosedFileNo)
    {
        var returnMessage = @"";
        var parameters = new[]
        {
            new SqlParameter("@p_EmployeeId", employeeId),
            new SqlParameter("@p_LeaveTypeId", leaveTypeId),
            new SqlParameter("@p_FromDate", fromDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@p_ToDate", toDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@p_Address", (object?)address ?? DBNull.Value),
            new SqlParameter("@p_Reason", (object?)reason ?? DBNull.Value),
            new SqlParameter("@p_FileName", (object?)enclosedFileName ?? DBNull.Value),
            new SqlParameter("@p_FileNo", (object?)enclosedFileNo ?? DBNull.Value),
        };
        try
        {
            var result = await context.Database.ExecuteSqlRawAsync(@"EXEC [Hrms].[USP_CreateLeaveRequest]
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
        var leaveRequestEntity = await context.TblLeaveRequests.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        //if (leaveRequestEntity == null)
        //{
        //    return new RecordNotFoundError($"Leave Request not found for the given Id");
        //}

        leaveRequestEntity.IsSubmitted = true;
        leaveRequestEntity.SubmittedOn = DateTime.Now;
        leaveRequestEntity.SubmittedBy = userId;

        return true;
    }
}
