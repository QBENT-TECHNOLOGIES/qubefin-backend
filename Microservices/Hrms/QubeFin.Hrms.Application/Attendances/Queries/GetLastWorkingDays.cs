using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetLastWorkingDaysQuery(Guid employeeId) : IRequest<Result<GetLastWorkingDaysResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetLastWorkingDaysQueryValidator : AbstractValidator<GetLastWorkingDaysQuery>
{
    public GetLastWorkingDaysQueryValidator()
    {
        RuleFor(v => v.employeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- LAST WORKING DAYS ---
public record GetLastWorkingDaysResponse(DateOnly? LastWorkingDay);
#endregion

#region --- HANDLER ---
internal sealed class GetLastWorkingDaysQueryHandler(QubeFin.Persistence.QubeFinDataContext context) : IRequestHandler<GetLastWorkingDaysQuery, Result<GetLastWorkingDaysResponse>>
{
    public async Task<Result<GetLastWorkingDaysResponse>> Handle(GetLastWorkingDaysQuery request, CancellationToken cancellationToken)
    {
        var result = await context.Database.SqlQuery<LastWorkingDateResult>($""" EXEC [Hrms].[USP_GetCurrentMonthLastWorkingDate] {request.employeeId}""")
            .ToListAsync(cancellationToken);

        var lastWorkingDate = result.FirstOrDefault();

        if (lastWorkingDate == null)
        {
            return Result.Fail("Last working date not found.");
        }

        return Result.Ok(new GetLastWorkingDaysResponse(lastWorkingDate.CalendarDate));
    }
}
#endregion