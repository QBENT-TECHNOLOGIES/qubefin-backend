using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.AttendanceMoralization.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Commands;

#region --- COMMAND ---
public record UpdateMoralizationCommand(Guid Id, List<EmployeeLosDetails> Updates) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class UpdateMoralizationCommandValidator : AbstractValidator<UpdateMoralizationCommand>
{
    public UpdateMoralizationCommandValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("Id is required.");
        RuleFor(x => x.Updates).NotNull().WithMessage("Updates records are required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateMoralizationCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMoralizationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateMoralizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingRecord = await context.TblEmployeeLops
             .Include(m => m.TblEmployeeLopDetails)
             .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (existingRecord?.TblEmployeeLopDetails is null)
            {
                return Result.Fail($"Record with Id {request.Id} not found.");
            }

            var detailsById = existingRecord.TblEmployeeLopDetails
                .ToDictionary(d => d.Id);

            foreach (var update in request.Updates)
            {
                if (!detailsById.TryGetValue(update.Id, out var existingDetail))
                {
                    return Result.Fail($"Detail with Id {update.Id} not found in the record.");
                }

                existingDetail.PayrollStatus = update.PayrollStatus;
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Moralization updated successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"{ex.Message}");
        }
    }
}
#endregion