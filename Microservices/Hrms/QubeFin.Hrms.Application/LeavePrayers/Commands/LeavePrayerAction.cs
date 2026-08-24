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
public record LeavePrayerActionCommand(Guid LeavePrayerId, bool IsApproved, bool IsRejected, Guid CurrentUserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class LeavePrayerActionCommandValidator : AbstractValidator<LeavePrayerActionCommand>
{
    public LeavePrayerActionCommandValidator()
    {
        RuleFor(v => v.LeavePrayerId).NotNull().WithMessage("Leave Prayer Id is required.");
        //RuleFor(v => v.RejectedReason).NotEmpty().WithMessage("Rejected Reason is required.").When(x => x.IsRejected == true);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class LeavePrayerActionCommandHandler(QubeFinDataContext context) : IRequestHandler<LeavePrayerActionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LeavePrayerActionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using (var cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[Hrms].[USP_LeavePrayerAction]";

                cmd.Parameters.Add(new SqlParameter("@p_LeavePrayerId", SqlDbType.UniqueIdentifier) { Value = request.LeavePrayerId });
                cmd.Parameters.Add(new SqlParameter("@p_IsApproved", SqlDbType.Bit) { Value = request.IsApproved });
                cmd.Parameters.Add(new SqlParameter("@p_IsRejected", SqlDbType.Bit) { Value = request.IsRejected });
                cmd.Parameters.Add(new SqlParameter("@p_CurrentUserId", SqlDbType.UniqueIdentifier) { Value = request.CurrentUserId });
                cmd.Parameters.Add(new SqlParameter("@p_RejectedReason", SqlDbType.VarChar) { Value = DBNull.Value });

                if (cmd.Connection.State != ConnectionState.Open)
                    await cmd.Connection.OpenAsync(cancellationToken);

                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

            return Result.Ok($"The leave prayer request has been {(request.IsApproved ? "approved" : request.IsRejected ? "rejected" : "recommended")} successfully.");
        }
        catch (SqlException ex)
        {
            return Result.Fail(
                string.IsNullOrWhiteSpace(ex.Message)
                    ? $"Failed to {(request.IsApproved ? "approve" : request.IsRejected ? "reject" : "recommend")}. Please try again later."
                    : ex.Message
            );
        }
    }
}
#endregion
