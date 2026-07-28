using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.Attendances.Commands;

public record SubmitAttendanceRegularizationCommand(Guid Id, Guid EmployeeId) : IRequest<Result<SubmitAttendanceRegularizationResponse>>;
public record SubmitAttendanceRegularizationResponse(bool Submitted);

internal sealed class SubmitAttendanceRegularizationCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<SubmitAttendanceRegularizationCommand, Result<SubmitAttendanceRegularizationResponse>>
{
    public async Task<Result<SubmitAttendanceRegularizationResponse>> Handle(SubmitAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        await attendanceRepository.SubmitAttendanceRegularization(request.Id, request.EmployeeId);
        return Result.Ok(new SubmitAttendanceRegularizationResponse(true));
    }
}
