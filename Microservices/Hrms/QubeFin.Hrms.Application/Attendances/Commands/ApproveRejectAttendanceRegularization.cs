using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.Attendances.Commands;

public record ApproveRejectAttendanceRegularizationCommand(Guid Id, bool IsApproved, Guid ActionBy) : IRequest<Result<ApproveRejectAttendanceRegularizationResponse>>;
public record ApproveRejectAttendanceRegularizationResponse(bool Processed);

internal sealed class ApproveRejectAttendanceRegularizationCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<ApproveRejectAttendanceRegularizationCommand, Result<ApproveRejectAttendanceRegularizationResponse>>
{
    public async Task<Result<ApproveRejectAttendanceRegularizationResponse>> Handle(ApproveRejectAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
       await attendanceRepository.ApproveRejectAttendanceRegularization(request.Id, request.IsApproved, request.ActionBy);
        return Result.Ok(new ApproveRejectAttendanceRegularizationResponse(true));
    }
}
