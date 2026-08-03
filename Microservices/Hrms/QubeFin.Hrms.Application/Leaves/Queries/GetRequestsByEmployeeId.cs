using FluentResults;
using MediatR;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Queries;

#region --- QUERY ---
public record GetRequestsByEmployeeIdQuery(int Year, Guid EmployeeId) : IRequest<Result<List<GetRequestsByEmployeeIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record class GetRequestsByEmployeeIdResponse(Guid Id, string LeaveType, DateTime FromDate, DateTime ToDate, int Totaldays, DateTime RequestDate, 
    string Address, string Reason, bool IsSubmitted, string CurrentStatus);
#endregion

#region --- HANDLER ---
internal sealed class GetLeaveRequestsByEmployeeIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetRequestsByEmployeeIdQuery, Result<List<GetRequestsByEmployeeIdResponse>>>
{
    public async Task<Result<List<GetRequestsByEmployeeIdResponse>>> Handle(GetRequestsByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var leaveRequests = await context.SP_GetEmployeeLeaveRequests(request.Year, request.EmployeeId);
        var response = leaveRequests.ConvertAll(m => new GetRequestsByEmployeeIdResponse(m.Id, m.LeaveType, m.FromDate, m.ToDate, m.TotalDays, m.RequestDate, m.Address, m.Reason, m.IsSubmitted, m.CurrentStatus));
        return Result.Ok(response);
    }
}
#endregion