using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Commands
{

    #region --- COMMAND ---
    public record CreateAttendanceCommand(Guid EmployeeId, TimeOnly time) : IRequest<Result<CreateAttendanceResponse>>;
    #endregion

    #region --- RESPONSE ---
    public record CreateAttendanceResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateAttendanceCommand, Result<CreateAttendanceResponse>>
    {
        public async Task<Result<CreateAttendanceResponse>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var todayAttendance = attendanceRepository.GetTodayAttendanceData(request.EmployeeId);

            if (todayAttendance is null)
            {
               
                var attendance = Attendance.MarkCheckIn(Guid.NewGuid(), request.EmployeeId, request.time, null, null, null);
            }
            else
            {
                //var attendance = Attendance.Update(Guid.NewGuid(), request.EmployeeId, request.time);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();

        }
    }
    #endregion
}
