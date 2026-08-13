using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.LeavePrayers.Queries;

#region --- QUERY ---
public record GetPrayerByEmployeeIdQuery(int Year, Guid EmployeeId) : IRequest<Result<List<GetPrayersByEmployeeIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record class GetPrayersByEmployeeIdResponse(Guid Id, string LeaveType, int PrayerDays, DateTime AppliedOn, string CurrentStatus);
#endregion

#region --- HANDLER ---
internal sealed class GetLeavePrayersByEmployeeIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetPrayerByEmployeeIdQuery, Result<List<GetPrayersByEmployeeIdResponse>>>
{
    public async Task<Result<List<GetPrayersByEmployeeIdResponse>>> Handle(GetPrayerByEmployeeIdQuery request, CancellationToken cancellationToken)
    {
        var leavePrayes = await context.TblLeavePrayers.Include(m => m.LeaveType).Where(m => m.EmployeeId == request.EmployeeId && m.CreatedOn.Year == request.Year).AsNoTracking().ToListAsync(cancellationToken);
        
        return Result.Ok(leavePrayes.Select(m => new GetPrayersByEmployeeIdResponse(m.Id, m.LeaveType.Title, m.PrayerDays, m.CreatedOn, m.CurrentStatus)).ToList());
    }
}
#endregion
