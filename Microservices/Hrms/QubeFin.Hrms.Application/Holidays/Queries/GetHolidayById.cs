using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetHolidayByIdQuery(Guid Id) : IRequest<Result<GetHolidayByIdResponse>>;

public record GetHolidayByIdResponse(Holiday Holiday);

internal sealed class GetHolidayByIdQueryHandler(IHolidayRepository holidayRepository)
    : IRequestHandler<GetHolidayByIdQuery, Result<GetHolidayByIdResponse>>
{
    public async Task<Result<GetHolidayByIdResponse>> Handle(GetHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        var holiday = await holidayRepository.GetByIdAsync(request.Id);
        return holiday is null
            ? new RecordNotFoundError("Holiday not found.")
            : Result.Ok(new GetHolidayByIdResponse(holiday));
    }
}
