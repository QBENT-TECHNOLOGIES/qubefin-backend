using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceByEmployeeQuery(Guid EmployeeId) : IRequest<Result<Attendance?>>;
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
internal sealed class GetAttendanceByEmployeeQueryHandler(IAttendanceRepository attendanceRepository) : 
        IRequestHandler<GetAttendanceByEmployeeQuery, Result<Attendance?>>
{
    public async Task<Result<Attendance?>> Handle(GetAttendanceByEmployeeQuery request, CancellationToken cancellationToken)
    {
       var attendance = await attendanceRepository.GetTodayAttendanceData(request.EmployeeId);
        return Result.Ok(attendance);
    }
}
#endregion
