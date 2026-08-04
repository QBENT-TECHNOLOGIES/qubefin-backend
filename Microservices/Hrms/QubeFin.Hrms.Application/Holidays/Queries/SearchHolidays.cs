using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record SearchHolidaysQuery(
    Guid? OrgUnitId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? SearchText,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<SearchHolidaysResponse>;

public class SearchHolidaysQueryValidator : AbstractValidator<SearchHolidaysQuery>
{
    public SearchHolidaysQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x).Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("From date must be earlier than or equal to to date.");
    }
}

public record SearchHolidaysResponse(IReadOnlyList<Holiday> Holidays, int TotalRecords);

internal sealed class SearchHolidaysQueryHandler(QubeFinDataContext context)
    : IRequestHandler<SearchHolidaysQuery, SearchHolidaysResponse>
{
    public async Task<SearchHolidaysResponse> Handle(SearchHolidaysQuery request, CancellationToken cancellationToken)
    {
        var query = context.TblHolidays.AsNoTracking().AsQueryable();

        if (request.OrgUnitId.HasValue && request.OrgUnitId != Guid.Empty)
        {
            query = query.Where(x => x.OrgUnitId == request.OrgUnitId.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.HolidayDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.HolidayDate <= request.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            query = query.Where(x => x.Description.Contains(searchText));
        }

        var totalRecords = await query.CountAsync(cancellationToken);
        var holidays = await query
            .OrderBy(x => x.HolidayDate)
            .ThenBy(x => x.Description)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new SearchHolidaysResponse(holidays.Select(x => x.ToDomain()).ToList(), totalRecords);
    }
}
