using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;


namespace QubeFin.Hrms.Application.Attendances.Commands
{

    #region --- COMMAND ---
    public record CreateAttendanceCommand(Guid EmployeeId, TimeOnly time, decimal Lat, decimal Long) : IRequest<Result<CreateAttendanceResponse>>;
    #endregion

    #region --- VALIDATOR ---
    public class CreateAttendanceCommandValidator : AbstractValidator<CreateAttendanceCommand>
    {
        public CreateAttendanceCommandValidator()
        {
            RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
            RuleFor(v => v.Lat).NotEmpty().WithMessage("Latitude is required");
            RuleFor(v => v.Long).NotEmpty().WithMessage("Longitude is required");
            RuleFor(v => v.time).NotEmpty().WithMessage("Time is required");
        }
    }
    #endregion

    #region --- RESPONSE ---
    public record CreateAttendanceResponse(bool success, string message);
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateAttendanceCommand, Result<CreateAttendanceResponse>>
    {
        public async Task<Result<CreateAttendanceResponse>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var employeeOrganization = await employeeRepository.GetEmloyeeOrganization(request.EmployeeId);
            if (employeeOrganization == null || employeeOrganization.OrganizationInfo.AttendanceInTime == null || employeeOrganization.OrganizationInfo.AttendanceOutTime == null)
            {
                throw new Exception("Organization In / Out Time not set.");
            }
            var org = employeeOrganization.OrganizationInfo;
            var todayAttendance = await attendanceRepository.GetTodayAttendanceData(request.EmployeeId);
            var expectedInTime = new TimeOnly(org.AttendanceInTime.Value.Hour, org.AttendanceInTime.Value.Minute);
            var expectedOutTime = new TimeOnly(org.AttendanceOutTime.Value.Hour, org.AttendanceOutTime.Value.Minute);
            var actualTime = new TimeOnly(request.time.Hour, request.time.Minute);
            if (todayAttendance is null)
            {
                var attendance = Attendance.MarkCheckIn(Guid.NewGuid(), request.EmployeeId, actualTime, null, org?.OrganizationUnitId, expectedInTime, expectedOutTime, request.Lat, request.Long, null, null, DateOnly.FromDateTime(DateTime.Now));
                await attendanceRepository.Create(attendance);
            }
            else
            {
                todayAttendance.MarchCheckOut(actualTime, expectedOutTime, request.Lat, request.Long);
                await attendanceRepository.Update(todayAttendance);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CreateAttendanceResponse(true, $"{(todayAttendance is null ? "Checked In Success" : "Checked Out Success")}"));
        }
    }
    #endregion
}
