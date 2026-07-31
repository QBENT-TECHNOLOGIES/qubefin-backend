using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using System.Data;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface ILeaveRepository
{
    Task<string> AddAsync(Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, string address, string reason, string enclosedFileName, string enclosedFileNo);
}

public class LeaveRepository(QubeFinDataContext context) : ILeaveRepository
{
    public async Task<string> AddAsync(Guid employeeId, Guid leaveTypeId, DateOnly fromDate, DateOnly toDate, string address, string reason, string enclosedFileName, string enclosedFileNo)
    {
        var returnMessage = @"";

        var parameters = new[]
        {
            new SqlParameter("@p_RequestId", Guid.Empty),
            new SqlParameter("@p_EmployeeId", employeeId),
            new SqlParameter("@p_LeaveTypeId", leaveTypeId),
            new SqlParameter("@p_FromDate", fromDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@p_ToDate", toDate.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@p_Reason", (object?)address ?? DBNull.Value),
            new SqlParameter("@p_Reason", (object?)reason ?? DBNull.Value),
            new SqlParameter("@p_FileName", (object?)enclosedFileName ?? DBNull.Value),
            new SqlParameter("@p_FileNo", (object?)enclosedFileNo ?? DBNull.Value),
        };

        try
        {
            var result = await context.Database.ExecuteSqlRawAsync(@"EXEC dbo.QSP_SaveLeaveRequest
              @p_RequestId,
              @p_EmployeeId,
              @p_LeaveTypeId,
              @p_FromDate,
              @p_ToDate,
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
}
