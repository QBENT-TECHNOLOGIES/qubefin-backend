using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetHolidayByIdQuery(Guid Id) : IRequest<Result<HolidayDetailResponse>>;

public record HolidayDetailResponse(
    Guid Id,
    DateOnly HolidayDate,
    string Description,
    string CreatedBy,
    DateTime CreatedOn,
    string? LastModifiedBy,
    DateTime? LastModifiedOn,
    List<OrgUnitDto> OrgUnits
);

public record OrgUnitDto(Guid Id, string Name);

internal sealed class GetHolidayByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetHolidayByIdQuery, Result<HolidayDetailResponse>> 
{
    public async Task<Result<HolidayDetailResponse>> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        var baseHoliday = await context.TblHolidays
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (baseHoliday == null) return Result.Fail("Holiday not found.");

        var relatedHolidays = await context.TblHolidays
            .AsNoTracking()
            .Where(x => x.HolidayDate == baseHoliday.HolidayDate && x.Description == baseHoliday.Description)
            .ToListAsync(cancellationToken);

        var orgUnitIds = relatedHolidays.Select(x => x.OrgUnitId).ToList();

        var orgUnits = await context.TblOrganizationUnits
            .AsNoTracking()
            .Where(ou => orgUnitIds.Contains(ou.Id))
            .Select(ou => new OrgUnitDto(ou.Id, ou.Name))
            .ToListAsync(cancellationToken);

        var creator = await context.TblUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == baseHoliday.CreatedBy, cancellationToken);

        var modifier = baseHoliday.LastModifiedBy.HasValue
            ? await context.TblUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == baseHoliday.LastModifiedBy.Value, cancellationToken)
            : null;

        var response = new HolidayDetailResponse(
            baseHoliday.Id,
            baseHoliday.HolidayDate,
            baseHoliday.Description,
            creator?.UserName ?? "System",
            baseHoliday.CreatedOn,
            modifier?.UserName,
            baseHoliday.LastModifiedOn,
            orgUnits 
        );

        return Result.Ok(response);
    }
}