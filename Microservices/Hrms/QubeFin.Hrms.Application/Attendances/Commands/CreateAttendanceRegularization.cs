using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;
using System.Data;
using System.Text.Json;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record CreateAttendanceRegularizationCommand(RegularizationRequest regularization, Guid EmployeeId, Guid UserId) : IRequest<Result<CreateAttendanceRegularizationResponse>>;
#endregion

#region --- VALIDATOR ---
public class CreateAttendanceRegularizationCommandValidator : AbstractValidator<CreateAttendanceRegularizationCommand>
{
    public CreateAttendanceRegularizationCommandValidator()
    {
        RuleFor(v => v.regularization).NotNull().WithMessage("Regularization request is required.");
        RuleFor(v => v.EmployeeId).NotEqual(Guid.Empty).WithMessage("Employee Id is required.");
        RuleFor(v => v.regularization.RegularizationDates).NotEmpty().WithMessage("Regularization date is required.");
        RuleFor(v => v.regularization.Reason).NotEmpty().WithMessage("Reason is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record CreateAttendanceRegularizationResponse(bool success, string message);
#endregion

#region --- HANDLER ---
internal sealed class CreateAttendanceRegularizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<CreateAttendanceRegularizationCommand, Result<CreateAttendanceRegularizationResponse>>
{
    public async Task<Result<CreateAttendanceRegularizationResponse>> Handle(CreateAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        string? attachment = null;
        if (request.regularization.Attachment != null && request.regularization.Attachment.Length > 0)
        {
            //attachment = Convert.ToBase64String(request.regularization.Attachment);
        }
        var regularizationDatesJson = JsonSerializer.Serialize(request.regularization.RegularizationDates.Select(d => d.ToString("yyyy-MM-dd")));
        var successParam = new SqlParameter("@Success", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
        {
            Direction = ParameterDirection.Output
        };
        var regularizationIdParam = new SqlParameter("@RegularizationId", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.Output
        };

        await context.Database.ExecuteSqlRawAsync(
            @"EXEC [Hrms].[USP_AppliedRegularization]
                @Id,
                @EmployeeId,
                @UserId,
                @RegularizationType,
                @RegularizationDates,
                @Reason,
                @Attachment,
                @Success OUTPUT,
                @Message OUTPUT,
                @RegularizationId OUTPUT",
        new SqlParameter("@Id", request.regularization.Id),
        new SqlParameter("@EmployeeId", request.EmployeeId),
        new SqlParameter("@UserId", request.UserId),
        new SqlParameter("@RegularizationType", request.regularization.RegularizationType),
        new SqlParameter("@RegularizationDates", regularizationDatesJson),
        new SqlParameter("@Reason", request.regularization.Reason),
        new SqlParameter("@Attachment", (object?)attachment ?? DBNull.Value),
        successParam,
        messageParam,
        regularizationIdParam);

        bool success = successParam.Value != DBNull.Value && (bool)successParam.Value;
        string message = messageParam.Value?.ToString() ?? string.Empty;
        Guid? regularizationId = regularizationIdParam.Value == DBNull.Value ? null : (Guid)regularizationIdParam.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateAttendanceRegularizationResponse(success, $"{message}"));
    }
}
#endregion
