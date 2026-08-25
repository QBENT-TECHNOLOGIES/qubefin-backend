using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- COMMAND --
public record UploadFitnessReportCommand(Guid employeeId, IFormFile? fitnessReportAttachment) : IRequest<Result<string>>;
#endregion
#region --- HANDLER ---
internal sealed class UploadFitnessReportCommandHandler(IFileStorageRepository fileStorageRepository, QubeFinDataContext context) : IRequestHandler<UploadFitnessReportCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadFitnessReportCommand request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var lastAttendance = await context.TblAttendances.AsNoTracking().Where(x => x.EmployeeId == request.employeeId && x.AttendanceDate < today)
            .OrderByDescending(x => x.AttendanceDate).FirstOrDefaultAsync(cancellationToken);

        if (lastAttendance == null)
        {
            return Result.Fail("Previous attendance not found.");
        }

        var leaveRequestEntity = await context.TblLeaveRequests.FirstOrDefaultAsync(m => m.EmployeeId == request.employeeId &&
                m.CurrentStatus.Trim() == "Approved" && m.ToDate > lastAttendance.AttendanceDate && string.IsNullOrEmpty(m.FitnessReportAttachment) && !m.IsFitnessReportApproved, cancellationToken);

        if (leaveRequestEntity == null)
        {
            return Result.Fail("No leave request found requiring a fitness report.");
        }

        if (request.fitnessReportAttachment == null || request.fitnessReportAttachment.Length == 0)
        {
            return Result.Fail("Fitness report attachment is required.");
        }

        var file = request.fitnessReportAttachment;
        await using var stream = file.OpenReadStream();

        var fileNo = await fileStorageRepository.UploadFileAsync( stream, file.FileName, file.ContentType ?? "application/octet-stream", cancellationToken);

        leaveRequestEntity.FitnessReportAttachment = fileNo;
        leaveRequestEntity.FitnessReportUploadOn = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok("Fitness report uploaded successfully.");
    }
}
#endregion