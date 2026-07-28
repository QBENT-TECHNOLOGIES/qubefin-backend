using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

#region --- QUERY ---
public record GetLeaveTypesByEmployeeIdQuery(Guid EmployeeId) : IRequest<Result<List<GetLeaveTypesByEmployeeIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetLeaveTypesByEmployeeIdResponse(Guid Id, string Title, string Alias, decimal LeaveEntitled, decimal LeaveTaken, decimal LeaveBalance);
#endregion

#region --- HANDLER ---
internal sealed class GetLeaveTypesByEmployeeIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetLeaveTypesByEmployeeIdQuery, Result<List<GetLeaveTypesByEmployeeIdResponse>>>
{
    public async Task<Result<List<GetLeaveTypesByEmployeeIdResponse>>> Handle(GetLeaveTypesByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var users = await context
            .TblLeaveTypes
            .AsNoTracking()
            .OrderBy(m => m.SeqNo)
            .Select(m => new GetLeaveTypesByEmployeeIdResponse(m.Id, m.Title, m.Alias, 0, 0, 0))
            .ToListAsync(cancellationToken);

        return Result.Ok(users);
    }
}
#endregion