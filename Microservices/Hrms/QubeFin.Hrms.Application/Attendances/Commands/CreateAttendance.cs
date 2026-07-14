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
                var employeeOrganization = await employeeRepository.GetEmloyeeOrganization(request.EmployeeId);
                var attendance = Attendance.MarkCheckIn(Guid.NewGuid(), request.EmployeeId, request.time, null, employeeOrganization?.OrganizationInfo?.OrganizationUnitId, employeeOrganization?.OrganizationInfo?.AttendanceInTime, employeeOrganization?.OrganizationInfo?.AttendanceOutTime, request.Lat, request.Long, null, null, DateOnly.FromDateTime(DateTime.Now));
                await attendanceRepository.Create(attendance);
            }
            else
            {
                todayAttendance.MarchCheckOut(request.time, request.Lat, request.Long);
                await attendanceRepository.Update(todayAttendance);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CreateAttendanceResponse(true));
        }
    }
    #endregion
}
