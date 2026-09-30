using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;


namespace QubeFin.Hrms.Application.Attendances.Commands
{

    #region --- COMMAND ---
    public record CreateAttendanceCommand(Guid EmployeeId, AttendancePunchRequest Request) : IRequest<Result<string>>;
    #endregion

    #region --- VALIDATOR ---
    public class CreateAttendanceCommandValidator : AbstractValidator<CreateAttendanceCommand>
    {
        public CreateAttendanceCommandValidator()
        {
            RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
            RuleFor(v => v.Request.OrganizationUnitId).NotEmpty().WithMessage("Organization Unit Id is required.");
            RuleFor(v => v.Request.Lat).NotEmpty().WithMessage("Latitude is required");
            RuleFor(v => v.Request.Long).NotEmpty().WithMessage("Longitude is required");
            RuleFor(v => v.Request.Time).NotEmpty().WithMessage("Time is required");
        }
    }
    #endregion

    #region --- HANDLER ---
         internal sealed class CreateAttendanceCommandHandler( IAttendanceRepository attendanceRepository, IFileStorageRepository fileStorageRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateAttendanceCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreateAttendanceCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var organization = await attendanceRepository.GetOrganization(request.OrganizationUnitId);
            if (organization == null || organization.AttendanceInTime == null || organization.AttendanceOutTime == null)
            {
                throw new Exception("Organization In / Out Time not set.");
            }
            var todayAttendance = await attendanceRepository.GetTodayAttendanceData(command.EmployeeId);
            var expectedInTime = new TimeOnly(organization.AttendanceInTime.Value.Hour, organization.AttendanceInTime.Value.Minute);
            var expectedOutTime = new TimeOnly(organization.AttendanceOutTime.Value.Hour, organization.AttendanceOutTime.Value.Minute);
            var actualTime = new TimeOnly(request.Time.Hour, request.Time.Minute);
            if (todayAttendance is null)
            {
                var startPhoto = (string?)null;
                if (request.StartMileagePhoto != null && request.StartMileagePhoto.Length > 0)
                {
                    var file = request.StartMileagePhoto;
                    await using var stream = file.OpenReadStream();
                    startPhoto = await fileStorageRepository.UploadFileAsync(stream, "Mileage/" + file.FileName, file.ContentType ?? "application/octet-stream", cancellationToken).ConfigureAwait(false);
                }

                var attendance = Attendance.MarkCheckIn(Guid.NewGuid(), command.EmployeeId, actualTime, null, request.OrganizationUnitId, expectedInTime, expectedOutTime, request.Lat, request.Long, null, null, DateOnly.FromDateTime(DateTime.Now), startMileage: request.StartMileage, startMileagePhoto: startPhoto);
                await attendanceRepository.Create(attendance);
            }
            else
            {
                var endPhoto = (string?)null;
                if (request.EndMileagePhoto != null && request.EndMileagePhoto.Length > 0)
                {
                    var file = request.EndMileagePhoto;
                    await using var stream = file.OpenReadStream();
                    endPhoto = await fileStorageRepository.UploadFileAsync(stream, "Mileage/" + file.FileName, file.ContentType ?? "application/octet-stream", cancellationToken).ConfigureAwait(false);
                }

                todayAttendance.MarchCheckOut(actualTime, expectedOutTime, request.Lat, request.Long, request.OrganizationUnitId, endMileage: request.EndMileage,
                    endMileagePhoto: endPhoto,
                    personalUseKm: request.PersonalUseKm);
                await attendanceRepository.Update(todayAttendance);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"{(todayAttendance is null ? "Checked In Success" : "Checked Out Success")}");
        }
       
    }
    #endregion
}
