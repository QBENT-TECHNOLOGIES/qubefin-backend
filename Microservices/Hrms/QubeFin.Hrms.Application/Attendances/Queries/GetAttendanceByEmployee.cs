using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

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
        var today = DateOnly.FromDateTime(DateTime.Today);

        var attendanceEntity = await context.TblAttendances.AsNoTracking().FirstOrDefaultAsync(m => m.EmployeeId == request.EmployeeId && m.AttendanceDate == today, cancellationToken);

        var lastAttendance = await context.TblAttendances.AsNoTracking().Where(m => m.EmployeeId == request.EmployeeId && m.AttendanceDate < today).OrderByDescending(m => m.AttendanceDate).FirstOrDefaultAsync(cancellationToken);

        var leaveTypes = new[] { "ML", "MML" };

        TblLeaveRequest? leaveEntity = null;

        if (lastAttendance != null)
        {
            leaveEntity = await context.TblLeaveRequests.Include(m => m.LeaveType).AsNoTracking()
                .Where(l =>
                    l.EmployeeId == request.EmployeeId &&
                    l.CurrentStatus == "Approved" &&
                    leaveTypes.Contains(l.LeaveType.Alias) &&
                    l.FromDate <= today &&
                    l.ToDate >= lastAttendance.AttendanceDate)
                .OrderByDescending(l => l.ToDate).FirstOrDefaultAsync(cancellationToken);
        }

        var result = new AttendanceResponse
        {
            IsFitnessReportRequired = leaveEntity != null && !leaveEntity.IsFitnessReportApproved,
            IsFitnessReportUploaded = leaveEntity != null && !string.IsNullOrWhiteSpace(leaveEntity.FitnessReportAttachment)
        };

        if (attendanceEntity != null)
        {
            result.Id = attendanceEntity.Id;
            result.EmployeeId = attendanceEntity.EmployeeId;
            result.OrganizationUnitId = attendanceEntity.OrganizationUnitId;

            result.AttendanceDate = attendanceEntity.AttendanceDate;

            result.ExpectedInTime = attendanceEntity.ExpectedInTime;
            result.ExpectedOutTime = attendanceEntity.ExpectedOutTime;

            result.ActualInTime = attendanceEntity.ActualInTime;
            result.ActualOutTime = attendanceEntity.ActualOutTime;

            result.IsEarlyLeave = attendanceEntity.IsEarlyLeave;
            result.IsLateEntry = attendanceEntity.IsLateEntry;

            result.InTimeLatitude = attendanceEntity.InTimeLatitude;
            result.InTimeLongitude = attendanceEntity.InTimeLongitude;

            result.OutTimeLatitude = attendanceEntity.OutTimeLatitude;
            result.OutTimeLongitude = attendanceEntity.OutTimeLongitude;
        }

        return Result.Ok(result);
    }
}
#endregion
