using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetMyHolidaysQuery(Guid EmployeeId) : IRequest<Result<IEnumerable<Holiday>>>;
internal sealed class GetMyHolidaysQueryHandler(IHolidayRepository holidayRepository)
    : IRequestHandler<GetMyHolidaysQuery, Result<IEnumerable<Holiday>>>
{
    public async Task<Result<IEnumerable<Holiday>>> Handle(GetMyHolidaysQuery request, CancellationToken cancellationToken)
    {
        var holidays = await holidayRepository.GetAllByEmployeeIdAsync(request.EmployeeId);
        return Result.Ok(holidays);
    }
}
