using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record CreateAttendanceRegularizationCommand(RegularizationRequest regularization, Guid EmployeeId) : IRequest<Result<CreateAttendanceRegularizationResponse>>;
#endregion

#region --- VALIDATOR ---
public class CreateAttendanceRegularizationCommandValidator : AbstractValidator<CreateAttendanceRegularizationCommand>
{
    public CreateAttendanceRegularizationCommandValidator()
    {
        RuleFor(v => v.regularization).NotNull().WithMessage("Regularization request is required.");
        RuleFor(v => v.EmployeeId).NotEqual(Guid.Empty).WithMessage("Employee Id is required.");
        RuleFor(v => v.regularization.RegularizationDate).NotEmpty().WithMessage("Regularization date is required.").
            Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Invalid regularizerization date.");
        RuleFor(v => v.regularization.Reason).NotEmpty().WithMessage("Reason is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record CreateAttendanceRegularizationResponse(bool success, string message);
#endregion

#region --- HANDLER ---
internal sealed class CreateAttendanceRegularizationCommandHandler(IAttendanceRepository attendanceRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateAttendanceRegularizationCommand, Result<CreateAttendanceRegularizationResponse>>
{
    public async Task<Result<CreateAttendanceRegularizationResponse>> Handle(CreateAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        string? attachment = null;
        if (request.regularization.Attachment != null && request.regularization.Attachment.Length > 0)
        {
            //attachment = Convert.ToBase64String(request.regularization.Attachment);
        }

        if (request.regularization.Id == Guid.Empty)
        {
            var regularization = AttendanceRegularization.CreateNew(Guid.NewGuid(), request.EmployeeId, request.regularization.RegularizationDate, request.regularization.Reason, attachment);
            await attendanceRepository.CreateRegularization(regularization);
        }
        else
        {
            await attendanceRepository.UpdateRegularization(request.regularization.Id, request.regularization.RegularizationDate, request.regularization.Reason, request.regularization.Attachment, attachment);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateAttendanceRegularizationResponse(true, $"Regularization applied successfully for : {request.regularization.RegularizationDate}"));
    }
}
#endregion
