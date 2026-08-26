using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Commands;

#region --- COMMAND ---
public record GenerateMoralizationCommand(int Month, int Year) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class GenerateMoralizationCommandValidator : AbstractValidator<GenerateMoralizationCommand>
{
    public GenerateMoralizationCommandValidator()
    {
        RuleFor(v => v.Month).InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");
        RuleFor(v => v.Year).GreaterThanOrEqualTo(2000).WithMessage("Year must be greater than or equal to 2000.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GenerateMoralizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateMoralizationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GenerateMoralizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var checkExistingMoralizationLocked = await context.TblEmployeeLops
                .AnyAsync(m => m.LopMonth == request.Month && m.LopYear == request.Year && m.IsLocked, cancellationToken);
            if (checkExistingMoralizationLocked)
            {
                return Result.Fail($"Moralization for {request.Month}/{request.Year} is already locked.");
            }
            var parameters = new[]
            {
                new SqlParameter("@p_Year", request.Year),
                new SqlParameter("@p_Month", request.Month),
            };
            var result = await context.Database.ExecuteSqlRawAsync(@"EXEC [Hrms].[USP_EmployeeLOPCalculation]
              @p_Year,
              @p_Month", parameters);
            return Result.Ok($"Moralization generated successfully for {request.Month}/{request.Year}");
        }
        catch (SqlException sqlEx)
        {
            return Result.Fail(sqlEx.Errors[0].Message);
        }
        catch (Exception ex)
        {
            return Result.Fail($"{ex.Message}");
        }
    }
}
#endregion
