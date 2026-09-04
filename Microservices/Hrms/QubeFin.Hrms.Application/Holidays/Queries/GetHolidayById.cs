using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetHolidayByIdQuery(DateOnly Holiday) : IRequest<Result<HolidayDetailResponse>>;

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
        var holidays = await context.TblHolidays
            .AsNoTracking()
            .Where(x => x.HolidayDate == request.Holiday)
            .ToListAsync(cancellationToken);

        if (holidays == null || !holidays.Any()) return Result.Fail("Holiday not found.");

        var orgUnitIds = holidays.Select(x => x.OrgUnitId).Distinct().ToList();
        var orgUnits = await context.TblOrganizationUnits
            .AsNoTracking()
            .Where(ou => orgUnitIds.Contains(ou.Id))
            .Select(ou => new OrgUnitDto(ou.Id, ou.Name))
            .ToListAsync(cancellationToken);

        var auditInfo = await context.TblUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == holidays.First().CreatedBy || u.Id == holidays.First().LastModifiedBy, cancellationToken);

        var response = new HolidayDetailResponse(
            holidays.First().Id,
            holidays.First().HolidayDate,
            holidays.First().Description,
            auditInfo?.UserName ?? string.Empty,
            holidays.First().CreatedOn,
            auditInfo?.UserName ?? string.Empty,
            holidays.First().LastModifiedOn,
            orgUnits
        );
        return Result.Ok(response);
    }
}