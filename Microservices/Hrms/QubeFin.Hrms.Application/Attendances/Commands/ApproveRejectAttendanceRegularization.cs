using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record ApproveRejectAttendanceRegularizationCommand(Guid Id, bool IsApproved, Guid ActionBy) : IRequest<Result<ApproveRejectAttendanceRegularizationResponse>>;
#endregion

#region --- VALIDATOR ---
public class ApproveRejectAttendanceRegularizationCommandValidator : AbstractValidator<ApproveRejectAttendanceRegularizationCommand>
{
    public ApproveRejectAttendanceRegularizationCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().WithMessage("Regularization Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record ApproveRejectAttendanceRegularizationResponse(bool success, string message);
#endregion

#region --- HANDLER ---

internal sealed class ApproveRejectAttendanceRegularizationCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<ApproveRejectAttendanceRegularizationCommand, Result<ApproveRejectAttendanceRegularizationResponse>>
{
    public async Task<Result<ApproveRejectAttendanceRegularizationResponse>> Handle(ApproveRejectAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
       //await attendanceRepository.ApproveRejectAttendanceRegularization(request.Id, request.IsApproved, request.ActionBy);
        return Result.Ok(new ApproveRejectAttendanceRegularizationResponse(true, $"Regularization { (request.IsApproved ? "Approved" : "Rejected") } successfully"));
    }
}
#endregion