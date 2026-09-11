using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Hrms.Application.Holidays.Commands;
public record UpdateHolidayCommand(
    List<Guid> OrgUnitIds,
    DateOnly HolidayDate,
    string Description,
    Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateHolidayCommandValidator : AbstractValidator<UpdateHolidayCommand>
{
    public UpdateHolidayCommandValidator()
    {
        RuleFor(x => x.HolidayDate).NotEmpty().WithMessage("Holiday date is required.");
        RuleFor(x => x.OrgUnitIds).NotEmpty().WithMessage("Please select at least one organization unit.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(50).WithMessage("Description cannot exceed 50 characters.");
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
internal sealed class UpdateHolidayCommandHandler(QubeFinDataContext context)
    : IRequestHandler<UpdateHolidayCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
    {
        var holidays = await context.TblHolidays
            .AsNoTracking()
            .Where(x => x.HolidayDate == request.HolidayDate).ToListAsync(cancellationToken);

        if (holidays == null || !holidays.Any())
        {
            return new RecordNotFoundError($"Holiday not found for date {request.HolidayDate}.");
        }

        var existingOrgUnitIds = holidays.Select(x => x.OrgUnitId).ToList();

        var toDelete = holidays.Where(x => !request.OrgUnitIds.Contains(x.OrgUnitId)).ToList();
        if (toDelete.Any())
        {
            context.TblHolidays.RemoveRange(toDelete);
        }

        var toUpdate = holidays.Where(x => request.OrgUnitIds.Contains(x.OrgUnitId)).ToList();
        foreach (var item in toUpdate)
        {
            item.HolidayDate = request.HolidayDate;
            item.Description = request.Description.Trim();
            item.LastModifiedBy = request.ModifiedBy;
            item.LastModifiedOn = DateTime.Now;

            context.TblHolidays.Update(item);
        }

        var toAddIds = request.OrgUnitIds.Where(id => !existingOrgUnitIds.Contains(id)).ToList();
        foreach (var orgUnitId in toAddIds)
        {
            var newEntity = new TblHoliday
            {
                Id = Guid.NewGuid(),
                OrgUnitId = orgUnitId,
                HolidayDate = request.HolidayDate,
                Description = request.Description.Trim(),
                CreatedBy = request.ModifiedBy,
                CreatedOn = DateTime.Now
            };
            await context.TblHolidays.AddAsync(newEntity, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Ok($"{request.HolidayDate.ToString("dd-MM-yyyy")} Holiday updated successfully.");
    }
}