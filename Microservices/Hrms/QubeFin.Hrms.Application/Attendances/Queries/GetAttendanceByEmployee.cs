using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceByEmployeeQuery(Guid EmployeeId) : IRequest<Result<AttendanceResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetAttendanceByEmployeeQueryValidator : AbstractValidator<GetAttendanceByEmployeeQuery>
{
    public GetAttendanceByEmployeeQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetAttendanceByEmployeeQueryHandler(QubeFinDataContext context) :  IRequestHandler<GetAttendanceByEmployeeQuery, Result<AttendanceResponse>>
{
    public async Task<Result<AttendanceResponse>> Handle(GetAttendanceByEmployeeQuery request,CancellationToken cancellationToken)
    {
        var result = new AttendanceResponse();
        bool isFitnessReportRequired = false;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var leaveTypes = new List<string> { "ML", "MML" };

        var attendanceEntity = await context.TblAttendances.AsNoTracking().FirstOrDefaultAsync(m => m.EmployeeId == request.EmployeeId && m.AttendanceDate == today,cancellationToken);

        var lastAttendance = await context.TblAttendances.AsNoTracking().Where(m => m.EmployeeId == request.EmployeeId && m.AttendanceDate < today).OrderByDescending(m => m.AttendanceDate).FirstOrDefaultAsync(cancellationToken);
        
        var leaveTypeList = await context.TblLeaveTypes.AsNoTracking().Where(m => leaveTypes.Contains(m.Alias)).Select(m => m.Id).ToListAsync();
        if (lastAttendance != null)
        {
            isFitnessReportRequired = await context.TblLeaveRequests.AnyAsync(l => l.EmployeeId == request.EmployeeId && l.CurrentStatus == "Approved" && leaveTypeList.Contains(l.LeaveTypeId) && l.ToDate > lastAttendance.AttendanceDate && !l.IsFitnessReportApproved, cancellationToken);
        }

        if(attendanceEntity == null)
        {
            result = new AttendanceResponse
            {
                IsFitnessReportRequired = isFitnessReportRequired,
            };
        }
        else
        {
            result = new AttendanceResponse
            {
                Id = attendanceEntity.Id,
                EmployeeId = attendanceEntity.EmployeeId,
                OrganizationUnitId = attendanceEntity.OrganizationUnitId,

                AttendanceDate = attendanceEntity.AttendanceDate,

                ExpectedInTime = attendanceEntity.ExpectedInTime,
                ExpectedOutTime = attendanceEntity.ExpectedOutTime,

                ActualInTime = attendanceEntity.ActualInTime,
                ActualOutTime = attendanceEntity.ActualOutTime,

                IsFitnessReportRequired = isFitnessReportRequired,

                IsEarlyLeave = attendanceEntity.IsEarlyLeave,
                IsLateEntry = attendanceEntity.IsLateEntry,

                InTimeLatitude = attendanceEntity.InTimeLatitude,
                InTimeLongitude = attendanceEntity.InTimeLongitude,

                OutTimeLatitude = attendanceEntity.OutTimeLatitude,
                OutTimeLongitude = attendanceEntity.OutTimeLongitude
            };

        }

        return Result.Ok(result);
    }
}
#endregion
