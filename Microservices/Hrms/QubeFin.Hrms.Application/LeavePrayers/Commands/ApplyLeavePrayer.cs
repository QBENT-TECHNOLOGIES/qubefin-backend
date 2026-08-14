using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.LeavePrayers.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using System.Data;

namespace QubeFin.Hrms.Application.LeavePrayers.Commands;

#region --- COMMAND ---
public record ApplyLeavePrayerCommand(LeavePrayerRequest prayer, Guid EmployeeId, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class ApplyLeavePrayerCommandValidator : AbstractValidator<ApplyLeavePrayerCommand>
{
    public ApplyLeavePrayerCommandValidator()
    {
        RuleFor(v => v.prayer).NotNull().WithMessage("Regularization request is required.");
        RuleFor(v => v.EmployeeId).NotEqual(Guid.Empty).WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class ApplyLeavePrayerCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork, IFileStorageRepository fileStorageRepository) : IRequestHandler<ApplyLeavePrayerCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ApplyLeavePrayerCommand request, CancellationToken cancellationToken)
    {
        string? attachment = null;
        if (request.prayer.Attachment != null && request.prayer.Attachment.Length > 0)
        {
            try
            {
                var file = request.prayer.Attachment;
                await using var stream = file.OpenReadStream();
                attachment = await fileStorageRepository.UploadFileAsync(stream, file.FileName, file.ContentType ?? "application/octet-stream", cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                attachment = null;
            }
        }
        var successParam = new SqlParameter("@Success", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
        {
            Direction = ParameterDirection.Output
        };
        var prayerIdParam = new SqlParameter("@PrayerId", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.Output
        };

        await context.Database.ExecuteSqlRawAsync(
            @"EXEC [Hrms].[USP_LeavePrayerApplied]
                @Id,
                @EmployeeId,
                @UserId,
                @LeaveTypeId ,
                @PrayerDays,
                @Attachment,
                @Remarks,
                @Success OUTPUT,
                @Message OUTPUT,
                @PrayerId OUTPUT",
        new SqlParameter("@Id", request.prayer.Id),
        new SqlParameter("@EmployeeId", request.EmployeeId),
        new SqlParameter("@UserId", request.UserId),
        new SqlParameter("@LeaveTypeId", request.prayer.LeaveTypeId),
        new SqlParameter("@PrayerDays", request.prayer.PrayerDays),
        new SqlParameter("@Attachment", (object?)attachment ?? DBNull.Value),
        new SqlParameter("@Remarks", (object?)request.prayer.Remarks ?? DBNull.Value),
        successParam,
        messageParam,
        prayerIdParam);

        bool success = successParam.Value != DBNull.Value && (bool)successParam.Value;
        string message = messageParam.Value?.ToString() ?? string.Empty;

        Guid? prayerId = prayerIdParam.Value == DBNull.Value ? null : (Guid)prayerIdParam.Value;

        if (!success)
        {
            return Result.Fail(message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(message);
    }
}
#endregion