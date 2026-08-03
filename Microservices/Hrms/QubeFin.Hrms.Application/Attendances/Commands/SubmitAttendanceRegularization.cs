using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record SubmitAttendanceRegularizationCommand(RegularizationSubmit submit, Guid EmployeeId) : IRequest<Result<SubmitAttendanceRegularizationResponse>>;
#endregion

#region --- VALIDATOR ---
public class SubmitAttendanceRegularizationCommandValidator : AbstractValidator<SubmitAttendanceRegularizationCommand>
{
    public SubmitAttendanceRegularizationCommandValidator()
    {
        RuleFor(v => v.submit.Id).NotNull().WithMessage("Regularization Id is required.");
        RuleFor(v => v.submit.Decision).NotEmpty().WithMessage("Regularization Decision is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record SubmitAttendanceRegularizationResponse(bool success, string message);
#endregion

#region --- HANDLER ---

internal sealed class SubmitAttendanceRegularizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<SubmitAttendanceRegularizationCommand, Result<SubmitAttendanceRegularizationResponse>>
{
    public async Task<Result<SubmitAttendanceRegularizationResponse>> Handle(SubmitAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                @"EXEC [Hrms].[USP_RegularizationAction]
                    @Id,
                    @EmployeeId,
                    @Decision,
                    @Remarks",
            new SqlParameter("@Id", request.submit.Id),
            new SqlParameter("@EmployeeId", request.EmployeeId),
            new SqlParameter("@Decision", request.submit.Decision),
            new SqlParameter("@Remarks", request.submit.Remarks));

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new SubmitAttendanceRegularizationResponse(true, $"Regularization {request.submit.Decision} successfully"));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
#endregion