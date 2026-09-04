using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Commands;

public record UpdateHolidayCommand(
    Guid Id,
    List<Guid> OrgUnitIds,
    DateOnly HolidayDate,
    string Description,
    Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateHolidayCommandValidator : AbstractValidator<UpdateHolidayCommand>
{
    public UpdateHolidayCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OrgUnitIds).NotEmpty().WithMessage("Please select at least one organization unit.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
internal sealed class UpdateHolidayCommandHandler(QubeFinDataContext context)
    : IRequestHandler<UpdateHolidayCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
    {
        var baseHoliday = await context.TblHolidays
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (baseHoliday == null)
        {
            return new RecordNotFoundError("Holiday not found.");
        }

        var existingGroup = await context.TblHolidays
            .Where(x => x.HolidayDate == baseHoliday.HolidayDate && x.Description == baseHoliday.Description)
            .ToListAsync(cancellationToken);

        var existingOrgUnitIds = existingGroup.Select(x => x.OrgUnitId).ToList();

        var toDelete = existingGroup.Where(x => !request.OrgUnitIds.Contains(x.OrgUnitId)).ToList();
        if (toDelete.Any())
        {
            context.TblHolidays.RemoveRange(toDelete);
        }

        var toUpdate = existingGroup.Where(x => request.OrgUnitIds.Contains(x.OrgUnitId)).ToList();
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
                CreatedBy = baseHoliday.CreatedBy,
                CreatedOn = baseHoliday.CreatedOn,
                LastModifiedBy = request.ModifiedBy,
                LastModifiedOn = DateTime.Now
            };
            await context.TblHolidays.AddAsync(newEntity, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{request.HolidayDate.ToString("dd-MM-yyyy")} Holiday updated successfully.");
    }
}