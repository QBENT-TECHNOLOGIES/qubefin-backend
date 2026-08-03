using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using System.Data;
namespace QubeFin.Hrms.Application.Leaves.Commands;


#region --- COMMAND --
public record LeaveRequestActionCommand(Guid LeaveRequestId, bool IsApproved, bool IsRejected, Guid CurrentUserId, string? RejectedReason) : IRequest<Result<LeaveRequestActionResponse>>;
#endregion

#region --- VALIDATOR ---
public class LeaveRequestActionCommandValidator : AbstractValidator<LeaveRequestActionCommand>
{
    public LeaveRequestActionCommandValidator()
    {
        RuleFor(v => v.LeaveRequestId).NotNull().WithMessage("Leave Request Id is required.");
        RuleFor(v => v.RejectedReason).NotEmpty().WithMessage("Rejected Reason is required.").When(x => x.IsRejected == true);
    }
}
#endregion

#region --- RESPONSE ---
public record LeaveRequestActionResponse(bool success, string message);
#endregion

#region --- HANDLER ---
internal sealed class LeaveRequestActionCommandHandler(QubeFinDataContext context) : IRequestHandler<LeaveRequestActionCommand, Result<LeaveRequestActionResponse>>
{
    public async Task<Result<LeaveRequestActionResponse>> Handle(LeaveRequestActionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[Hrms].[QSP_SaveLeaveRequestAction]";

                cmd.Parameters.Add(new SqlParameter("@p_LeaveRequestId", SqlDbType.UniqueIdentifier) { Value = request.LeaveRequestId });
                cmd.Parameters.Add(new SqlParameter("@p_IsApproved", SqlDbType.Bit) { Value = request.IsApproved });
                cmd.Parameters.Add(new SqlParameter("@p_IsRejected", SqlDbType.Bit) { Value = request.IsRejected });
                cmd.Parameters.Add(new SqlParameter("@p_CurrentUserId", SqlDbType.UniqueIdentifier) { Value = request.CurrentUserId });
                cmd.Parameters.Add(new SqlParameter("@p_RejectedReason", SqlDbType.VarChar) { Value = (object?)request.RejectedReason ?? DBNull.Value });

                if (cmd.Connection.State != ConnectionState.Open)
                    await cmd.Connection.OpenAsync(cancellationToken);

                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            return Result.Ok(new LeaveRequestActionResponse(true, $"The leave request has been {(request.IsApproved  ? "approved" : request.IsRejected ? "rejected" : "recommended")} successfully."));
        }
        catch
        {
            return Result.Ok(new LeaveRequestActionResponse(false, $"Faild to  {(request.IsApproved ? "approve" : request.IsRejected ? "reject" : "recommend")}. Please try again later."));
        }
    }
}
#endregion