using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetHolidayByIdQuery(Guid Id) : IRequest<Result<Holiday>>;
internal sealed class GetHolidayByIdQueryHandler(IHolidayRepository holidayRepository)
    : IRequestHandler<GetHolidayByIdQuery, Result<Holiday>>
{
    public async Task<Result<Holiday>> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        var holiday = await holidayRepository.GetByIdAsync(request.Id);
        return holiday is null
            ? new RecordNotFoundError("Holiday not found.")
            : Result.Ok(holiday);
    }
}
