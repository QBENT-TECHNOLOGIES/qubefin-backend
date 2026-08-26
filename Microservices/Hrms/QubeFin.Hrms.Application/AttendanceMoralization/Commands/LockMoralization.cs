using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

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
internal sealed class LockMoralizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<LockMoralizationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LockMoralizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingRecords = await context.TblEmployeeLops
                 .Where(m => m.LopMonth == request.month && m.LopYear == request.year)
                 .ToListAsync(cancellationToken);
            if (existingRecords == null)
            {
                return Result.Fail($"No records found for {request.month}/{request.year}");
            }

            foreach (var record in existingRecords)
            {
                record.IsLocked = true;
                context.TblEmployeeLops.Update(record);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Moralization locked successfully for {request.month}/{request.year}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"{ex.Message}");
        }
    }
}
#endregion
