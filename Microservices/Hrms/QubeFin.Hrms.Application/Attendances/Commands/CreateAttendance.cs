using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Commands
{

    #region --- COMMAND ---
    public record CreateAttendanceCommand(Guid EmployeeId, TimeOnly time, decimal Lat, decimal Long) : IRequest<Result<CreateAttendanceResponse>>;
    #endregion

    #region --- RESPONSE ---
    public record CreateAttendanceResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateAttendanceCommand, Result<CreateAttendanceResponse>>
    {
        public async Task<Result<CreateAttendanceResponse>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var todayAttendance = await attendanceRepository.GetTodayAttendanceData(request.EmployeeId);

            if (todayAttendance is null)
            {
               var employeeOrganization = await employeeRepository.GetEmployeeOrganization(request.EmployeeId);
               var attendance = Attendance.MarkCheckInCheckOut(Guid.NewGuid(), request.EmployeeId, request.time, null, employeeOrganization?.OrganizationUnitId, employeeOrganization?.AttendanceInTime, employeeOrganization?.AttendanceOutTime, request.Lat, request.Long, null, null);
                await attendanceRepository.Create(attendance);
            }
            else
            {
                var attendance = Attendance.MarkCheckInCheckOut(todayAttendance.Id, request.EmployeeId, todayAttendance.ActualInTime, request.time, todayAttendance.OrganizationUnitId, todayAttendance.ExpectedInTime, todayAttendance.ExpectedOutTime, todayAttendance.InTimeLat, todayAttendance.InTimeLong, todayAttendance.OutTimeLat, todayAttendance.OutTimeLong);
                await attendanceRepository.Update(attendance);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CreateAttendanceResponse(true));

        }
    }
    #endregion
}
