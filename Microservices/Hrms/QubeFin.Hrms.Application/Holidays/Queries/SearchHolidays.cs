using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record SearchHolidaysQuery(int Year) : IRequest<Result<List<SearchHolidaysResponse>>>;

public class SearchHolidaysQueryValidator : AbstractValidator<SearchHolidaysQuery>
{
    public SearchHolidaysQueryValidator()
    {
        RuleFor(x => x.Year).GreaterThan(2000).WithMessage("Please provide a valid year.");
    }
}

public record SearchHolidaysResponse(Guid Id, DateOnly HolidayDate, string Description);

internal sealed class SearchHolidaysQueryHandler(QubeFinDataContext context)
    : IRequestHandler<SearchHolidaysQuery, Result<List<SearchHolidaysResponse>>>
{
    public async Task<Result<List<SearchHolidaysResponse>>> Handle(SearchHolidaysQuery request, CancellationToken cancellationToken)
    {
       
        var rawHolidays = await context.TblHolidays
            .AsNoTracking()
            .Where(x => x.HolidayDate.Year == request.Year)
            .ToListAsync(cancellationToken);

        
        var holidays = rawHolidays
            .GroupBy(x => new { x.HolidayDate, x.Description })
            .Select(g => new SearchHolidaysResponse(
                g.First().Id, 
                g.Key.HolidayDate,
                g.Key.Description
            ))
            .OrderBy(x => x.HolidayDate)
            .ToList();

        return Result.Ok(holidays);
    }
}