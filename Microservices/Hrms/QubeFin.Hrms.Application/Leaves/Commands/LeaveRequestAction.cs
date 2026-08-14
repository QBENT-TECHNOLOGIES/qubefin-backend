using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using System.Data;
namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- COMMAND --
public record LeaveRequestActionCommand(Guid LeaveRequestId, bool IsApproved, bool IsRejected, Guid CurrentUserId, string? RejectedReason) : IRequest<Result<string>>;
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

#region --- HANDLER ---
internal sealed class LeaveRequestActionCommandHandler(QubeFinDataContext context) : IRequestHandler<LeaveRequestActionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LeaveRequestActionCommand request, CancellationToken cancellationToken)
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

            return Result.Ok($"The leave request has been {(request.IsApproved ? "approved" : request.IsRejected ? "rejected" : "recommended")} successfully.");
        }
        catch
        {
            return Result.Fail($"Failed to {(request.IsApproved ? "approve" : request.IsRejected ? "reject" : "recommend")} the leave request. Please try again later.");
        }
    }
}
#endregion