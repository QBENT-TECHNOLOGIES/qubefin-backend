using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

#region --- QUERY ---
public record GetLeaveTypesQuery : IRequest<Result<List<GetLeaveTypesResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetLeaveTypesResponse(Guid Id, string Title, string Alias, bool IsPrayerable, bool IsConvertible, bool IsEncashable, int NoOfDaysEntitled, int? NoOfDaysCapped,
        int MaxContinuousDays, bool ApplicableAfterProbation, bool IsMonthlyCredit);
#endregion

#region --- HANDLER ---
internal sealed class GetLeaveTypesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetLeaveTypesQuery, Result<List<GetLeaveTypesResponse>>>
{
    public async Task<Result<List<GetLeaveTypesResponse>>> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        var users = await context
            .TblLeaveTypes
            .AsNoTracking()
            .OrderBy(m => m.SeqNo)
            .Select(m => new GetLeaveTypesResponse(m.Id, m.Title, m.Alias, m.IsPrayerable, m.IsConvertible, m.IsEncashable, m.NoOfDaysEntitled, m.NoOfDaysCapped, m.MaxContinuousDays, m.ApplicableAfterProbation, m.IsMonthlyCredit))
            .ToListAsync(cancellationToken);

        return Result.Ok(users);
    }
}
#endregion