using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceByEmployeeQuery(Guid EmployeeId) : IRequest<Result<GetByIdResponse>>;
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
#region --- RESPONSE ---
public record GetByIdResponse(Attendance? attendance);
#endregion

#region --- HANDLER ---
internal sealed class GetAttendanceByEmployeeQueryHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<GetAttendanceByEmployeeQuery, Result<GetByIdResponse>>
{
    public async Task<Result<GetByIdResponse>> Handle(GetAttendanceByEmployeeQuery request, CancellationToken cancellationToken)
    {
       var attendance = await attendanceRepository.GetTodayAttendanceData(request.EmployeeId);
        return new GetByIdResponse(attendance);
    }
}
#endregion
