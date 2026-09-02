using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;


namespace QubeFin.Hrms.Application.Attendances.Commands
{

    #region --- COMMAND ---
    public record CreateAttendanceCommand(Guid EmployeeId, Guid OrganizationUnitId, TimeOnly time, decimal Lat, decimal Long) : IRequest<Result<string>>;
    #endregion

    #region --- VALIDATOR ---
    public class CreateAttendanceCommandValidator : AbstractValidator<CreateAttendanceCommand>
    {
        public CreateAttendanceCommandValidator()
        {
            RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
            RuleFor(v => v.OrganizationUnitId).NotEmpty().WithMessage("Organization Unit Id is required.");
            RuleFor(v => v.Lat).NotEmpty().WithMessage("Latitude is required");
            RuleFor(v => v.Long).NotEmpty().WithMessage("Longitude is required");
            RuleFor(v => v.time).NotEmpty().WithMessage("Time is required");
        }
    }
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateAttendanceCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var organization = await attendanceRepository.GetOrganization(request.OrganizationUnitId);
            if (organization == null || organization.AttendanceInTime == null || organization.AttendanceOutTime == null)
            {
                throw new Exception("Organization In / Out Time not set.");
            }
            var todayAttendance = await attendanceRepository.GetTodayAttendanceData(request.EmployeeId);
            var expectedInTime = new TimeOnly(organization.AttendanceInTime.Value.Hour, organization.AttendanceInTime.Value.Minute);
            var expectedOutTime = new TimeOnly(organization.AttendanceOutTime.Value.Hour, organization.AttendanceOutTime.Value.Minute);
            var actualTime = new TimeOnly(request.time.Hour, request.time.Minute);
            if (todayAttendance is null)
            {
                var attendance = Attendance.MarkCheckIn(Guid.NewGuid(), request.EmployeeId, actualTime, null, request.OrganizationUnitId, expectedInTime, expectedOutTime, request.Lat, request.Long, null, null, DateOnly.FromDateTime(DateTime.Now));
                await attendanceRepository.Create(attendance);
            }
            else
            {
                todayAttendance.MarchCheckOut(actualTime, expectedOutTime, request.Lat, request.Long, request.OrganizationUnitId);
                await attendanceRepository.Update(todayAttendance);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"{(todayAttendance is null ? "Checked In Success" : "Checked Out Success")}");
        }
    }
    #endregion
}
