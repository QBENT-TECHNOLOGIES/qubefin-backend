using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using System.Data;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Commands;

#region --- COMMAND ---
public record LockMoralizationCommand(int month, int year) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class LockMoralizationCommandValidator : AbstractValidator<LockMoralizationCommand>
{
    public LockMoralizationCommandValidator()
    {
        RuleFor(v => v.month).InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");
        RuleFor(v => v.year).GreaterThanOrEqualTo(2000).WithMessage("Year must be greater than or equal to 2000.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class LockMoralizationCommandHandler(QubeFinDataContext context)
    : IRequestHandler<LockMoralizationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LockMoralizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var messageParam = new SqlParameter("@p_Message", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@p_Month", SqlDbType.Int) { Value = request.month },
                new SqlParameter("@p_Year", SqlDbType.Int) { Value = request.year },
                new SqlParameter("@p_UserId", SqlDbType.UniqueIdentifier) { Value = DBNull.Value },
                messageParam
            };
            await context.Database.ExecuteSqlRawAsync(
                @"EXEC [Hrms].[USP_EmployeeLOPLock]
                    @p_Month,
                    @p_Year,
                    @p_UserId,
                    @p_Message OUTPUT",
                parameters,
                cancellationToken);

            var message = messageParam.Value as string;
            if (!string.IsNullOrWhiteSpace(message))
            {
                return Result.Fail(message);
            }
            return Result.Ok($"Moralization locked successfully for {request.month}/{request.year}.");
        }
        catch (SqlException ex)
        {
            return Result.Fail(ex.Errors[0].Message);
        }
    }
}
#endregion
