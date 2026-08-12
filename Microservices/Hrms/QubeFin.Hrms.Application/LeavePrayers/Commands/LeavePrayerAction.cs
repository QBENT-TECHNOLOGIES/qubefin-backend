using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QubeFin.Hrms.Application.LeavePrayers.Commands;

#region --- COMMAND --
public record LeavePrayerActionCommand(Guid LeaveRequestId, bool IsApproved, bool IsRejected, Guid CurrentUserId) : IRequest<Result<LeavePrayerActionResponse>>;
#endregion

#region --- VALIDATOR ---
public class LeavePrayerActionCommandValidator : AbstractValidator<LeavePrayerActionCommand>
{
    public LeavePrayerActionCommandValidator()
    {
        RuleFor(v => v.LeaveRequestId).NotNull().WithMessage("Leave Request Id is required.");
        //RuleFor(v => v.RejectedReason).NotEmpty().WithMessage("Rejected Reason is required.").When(x => x.IsRejected == true);
    }
}
#endregion

#region --- RESPONSE ---
public record LeavePrayerActionResponse(bool success, string message);
#endregion

#region --- HANDLER ---
internal sealed class LeavePrayerActionCommandHandler(QubeFinDataContext context) : IRequestHandler<LeavePrayerActionCommand, Result<LeavePrayerActionResponse>>
{
    public async Task<Result<LeavePrayerActionResponse>> Handle(LeavePrayerActionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[Hrms].[USP_LeavePrayerAction]";

                cmd.Parameters.Add(new SqlParameter("@p_LeavePrayerId", SqlDbType.UniqueIdentifier) { Value = request.LeaveRequestId });
                cmd.Parameters.Add(new SqlParameter("@p_IsApproved", SqlDbType.Bit) { Value = request.IsApproved });
                cmd.Parameters.Add(new SqlParameter("@p_IsRejected", SqlDbType.Bit) { Value = request.IsRejected });
                cmd.Parameters.Add(new SqlParameter("@p_CurrentUserId", SqlDbType.UniqueIdentifier) { Value = request.CurrentUserId });
                cmd.Parameters.Add(new SqlParameter("@p_RejectedReason", SqlDbType.VarChar) { Value = DBNull.Value });

                if (cmd.Connection.State != ConnectionState.Open)
                    await cmd.Connection.OpenAsync(cancellationToken);

                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            return Result.Ok(new LeavePrayerActionResponse(true, $"The leave prayer request has been {(request.IsApproved ? "approved" : request.IsRejected ? "rejected" : "recommended")} successfully."));
        }
        catch
        {
            return Result.Ok(new LeavePrayerActionResponse(false, $"Faild to  {(request.IsApproved ? "approve" : request.IsRejected ? "reject" : "recommend")}. Please try again later."));
        }
    }
}
#endregion
