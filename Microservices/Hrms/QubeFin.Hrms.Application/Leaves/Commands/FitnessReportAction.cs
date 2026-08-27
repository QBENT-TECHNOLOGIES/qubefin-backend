using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Leaves.Commands;


#region --- COMMAND --
public record FitnessReportActionCommand(Guid LeaveRequestId, Guid userId) : IRequest<Result<string>>;
#endregion
#region --- HANDLER ---
internal sealed class FitnessReportActionCommandHandler(QubeFinDataContext context) : IRequestHandler<FitnessReportActionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(FitnessReportActionCommand request, CancellationToken cancellationToken)
    {
        var leaveRequestEntity = await context.TblLeaveRequests.FirstOrDefaultAsync(m => m.Id == request.LeaveRequestId,cancellationToken);

        if (leaveRequestEntity == null)
        {
            return Result.Fail("Leave request not found.");
        }

        if (string.IsNullOrEmpty(leaveRequestEntity.FitnessReportAttachment))
        {
            return Result.Fail("Fitness report has not been uploaded.");
        }

        if (leaveRequestEntity.IsFitnessReportApproved)
        {
            return Result.Fail("Fitness report is already approved.");
        }

        leaveRequestEntity.FitnessReportApprovedBy = request.userId;
        leaveRequestEntity.FitnessReportApprovedOn = DateTime.Now;
        leaveRequestEntity.IsFitnessReportApproved = true;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok("Fitness report approved successfully.");
    }
}
#endregion