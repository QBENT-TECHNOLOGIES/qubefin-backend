using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

#region --- QUERY ---
public record GetLeavePrayerTypeByEmployeeIdQuery(Guid EmployeeId) : IRequest<Result<List<GetLeavePrayerTypeByEmployeeIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetLeavePrayerTypeByEmployeeIdResponse(Guid LeaveTypeId, string Title, string Alias, decimal LeaveEntitled, decimal LeaveTaken, decimal LeaveBalance);
#endregion

#region --- HANDLER ---
internal sealed class GetLeavePrayerTypeByEmployeeIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetLeavePrayerTypeByEmployeeIdQuery, Result<List<GetLeavePrayerTypeByEmployeeIdResponse>>>
{
    public async Task<Result<List<GetLeavePrayerTypeByEmployeeIdResponse>>> Handle(GetLeavePrayerTypeByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var employeeEntity = await context.TblEmployees.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.EmployeeId) ?? throw new ArgumentException("Employee not found.");
        var leaveTypeEntities = await context.TblLeaveTypes.Include(m => m.TblLeaveTransactions).Where(m => m.IsPrayerable &&
                        m.Alias != (employeeEntity.Gender.Trim().ToLower() == "male" ? "MML" : "PL")).AsNoTracking().ToListAsync(cancellationToken);


        return Result.Ok(leaveTypeEntities.Select(m => new GetLeavePrayerTypeByEmployeeIdResponse(
            m.Id,
            m.Title,
            m.Alias,
            m.NoOfDaysEntitled,
            m.TblLeaveTransactions.Any() ? m.TblLeaveTransactions.Sum(m => m.LeaveDebit) : 0,
            (m.Alias == "PL" || m.Alias == "MML") && m.TblLeaveTransactions.Any(m => m.LeaveDebit > 0) ? 0 : m.TblLeaveTransactions.Any()
            ? m.NoOfDaysEntitled - m.TblLeaveTransactions.Sum(m => m.LeaveDebit) : m.NoOfDaysEntitled)).ToList());
    }
}
#endregion
