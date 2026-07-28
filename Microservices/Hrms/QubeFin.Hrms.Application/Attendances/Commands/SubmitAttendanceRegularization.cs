using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record SubmitAttendanceRegularizationCommand(Guid Id, Guid EmployeeId) : IRequest<Result<SubmitAttendanceRegularizationResponse>>;
#endregion

#region --- VALIDATOR ---
public class SubmitAttendanceRegularizationCommandValidator : AbstractValidator<SubmitAttendanceRegularizationCommand>
{
    public SubmitAttendanceRegularizationCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().WithMessage("Regularization Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record SubmitAttendanceRegularizationResponse(bool success, string message);
#endregion

#region --- HANDLER ---

internal sealed class SubmitAttendanceRegularizationCommandHandler(IAttendanceRepository attendanceRepository) : IRequestHandler<SubmitAttendanceRegularizationCommand, Result<SubmitAttendanceRegularizationResponse>>
{
    public async Task<Result<SubmitAttendanceRegularizationResponse>> Handle(SubmitAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        await attendanceRepository.SubmitAttendanceRegularization(request.Id, request.EmployeeId);
        return Result.Ok(new SubmitAttendanceRegularizationResponse(true, $"Regularization submited successfully"));
    }
}
#endregion