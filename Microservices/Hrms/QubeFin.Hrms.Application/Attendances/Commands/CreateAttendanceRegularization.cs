using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using System.Data;
using System.Text.Json;

namespace QubeFin.Hrms.Application.Attendances.Commands;

#region --- COMMAND ---
public record CreateAttendanceRegularizationCommand(RegularizationRequest regularization, Guid EmployeeId, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CreateAttendanceRegularizationCommandValidator : AbstractValidator<CreateAttendanceRegularizationCommand>
{
    public CreateAttendanceRegularizationCommandValidator()
    {
        RuleFor(v => v.regularization).NotNull().WithMessage("Regularization request is required.");
        RuleFor(v => v.EmployeeId).NotEqual(Guid.Empty).WithMessage("Employee Id is required.");
        RuleFor(v => v.regularization.RegularizationDates).NotEmpty().WithMessage("Regularization date is required.");
        RuleFor(v => v.regularization.RegularizationType).NotEmpty().WithMessage("Regularization type is required.");
        RuleFor(v => v.regularization.Reason).NotEmpty().When(r => r.regularization.RegularizationType == "ATTENDANCE").WithMessage("Reason is required for attendance regularization types.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CreateAttendanceRegularizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork, IFileStorageRepository fileStorageRepository) : 
    IRequestHandler<CreateAttendanceRegularizationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateAttendanceRegularizationCommand request, CancellationToken cancellationToken)
    {
        string? attachment = null;
        if (request.regularization.Attachment != null && request.regularization.Attachment.Length > 0)
        {
            try
            {
                var file = request.regularization.Attachment;
                await using var stream = file.OpenReadStream();
                attachment = await fileStorageRepository.UploadFileAsync(stream, file.FileName, file.ContentType ?? "application/octet-stream", cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                attachment = null;
            }
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
            @"EXEC [Hrms].[USP_RegularizationApplied]
                @Id,
                @EmployeeId,
                @UserId,
                @RegularizationType,
                @RegularizationDates,
                @Reason,
                @Attachment,
                @Remarks,
                @Success OUTPUT,
                @Message OUTPUT,
                @RegularizationId OUTPUT",
        new SqlParameter("@Id", request.regularization.Id),
        new SqlParameter("@EmployeeId", request.EmployeeId),
        new SqlParameter("@UserId", request.UserId),
        new SqlParameter("@RegularizationType", request.regularization.RegularizationType),
        new SqlParameter("@RegularizationDates", regularizationDatesJson),
        new SqlParameter("@Reason", (object?)request.regularization.Reason ?? DBNull.Value),
        new SqlParameter("@Attachment", (object?)attachment ?? DBNull.Value),
        new SqlParameter("@Remarks", (object?)request.regularization.Remarks ?? DBNull.Value),
        successParam,
        messageParam,
        regularizationIdParam);

        bool success = successParam.Value != DBNull.Value && (bool)successParam.Value;
        if (!success)
        {
            return Result.Fail(messageParam.Value?.ToString() ?? "An error occurred while processing the regularization request.");
        }
        string message = messageParam.Value?.ToString() ?? string.Empty;
        Guid? regularizationId = regularizationIdParam.Value == DBNull.Value ? null : (Guid)regularizationIdParam.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"{message}");
    }
}
#endregion
