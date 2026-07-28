using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

#region --- QUERY ---
public record GetLeaveTypesByEmployeeIdQuery(Guid EmployeeId) : IRequest<Result<List<GetLeaveTypesByEmployeeIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetLeaveTypesByEmployeeIdResponse(Guid LeaveTypeId, string Title, string Alias, decimal LeaveEntitled, decimal LeaveTaken, decimal LeaveBalance);
#endregion

#region --- HANDLER ---
internal sealed class GetLeaveTypesByEmployeeIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetLeaveTypesByEmployeeIdQuery, Result<List<GetLeaveTypesByEmployeeIdResponse>>>
{
    public async Task<Result<List<GetLeaveTypesByEmployeeIdResponse>>> Handle(GetLeaveTypesByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var leaveBalances = await context.Set<EmployeewiseLeaveTypeBalance>()
            .FromSqlInterpolated($@"EXEC [Hrms].[USP_GetLeaveTypesByEmployee] @p_EmployeeId = {request.EmployeeId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Ok(leaveBalances.Select(m => new GetLeaveTypesByEmployeeIdResponse(m.LeaveTypeId, m.Title, m.Alias, m.LeaveEntitled, m.LeaveTaken, m.LeaveBalance)).ToList());
    }
}
#endregion