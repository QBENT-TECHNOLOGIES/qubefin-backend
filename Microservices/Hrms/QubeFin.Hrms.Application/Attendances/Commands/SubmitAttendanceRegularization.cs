using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record SubmitAttendanceRegularizationCommand(RegularizationSubmit submit, Guid EmployeeId, Guid CurrentUserId) : IRequest<Result<string>>;
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

#region --- HANDLER ---

internal sealed class SubmitAttendanceRegularizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) :
    IRequestHandler<SubmitAttendanceRegularizationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SubmitAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        bool isApproved = request.submit.Decision.ToLower().Trim() == "approve";
        bool isRejected = request.submit.Decision.ToLower().Trim() == "reject";
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                @"EXEC [Hrms].[USP_RegularizationAction]
                    @Id,
                    @Decision,
                    @Remarks,
                    @CurrentUserId",
            new SqlParameter("@Id", request.submit.Id),
            new SqlParameter("@Decision", request.submit.Decision),
            new SqlParameter("@Remarks", request.submit.Remarks),
            new SqlParameter("@CurrentUserId", request.CurrentUserId));

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Regularization {request.submit.Decision} successfully");
        }
        catch (SqlException ex)
        {
            return Result.Fail(
                string.IsNullOrWhiteSpace(ex.Message)
                    ? $"Failed to {(isApproved ? "approve" : isRejected ? "reject" : "recommend")}. Please try again later."
                    : ex.Message
            );
        }
    }
}
#endregion