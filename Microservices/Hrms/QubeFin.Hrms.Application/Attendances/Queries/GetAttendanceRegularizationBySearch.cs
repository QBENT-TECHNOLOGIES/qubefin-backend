using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceRegularizationBySearch(Guid EmployeeId, AttendanceSearchRequest searchParam) : IRequest<GetAttendanceRegularizationBySearchResponse>;
#endregion

#region --- RESPONSE ---
public record GetAttendanceRegularizationBySearchResponse(IReadOnlyList<RegularizationSearchResult> results, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetRegularizationHistoryBySearchHandler(QubeFinDataContext context) : IRequestHandler<GetAttendanceRegularizationBySearch, GetAttendanceRegularizationBySearchResponse>
{
    public async Task<GetAttendanceRegularizationBySearchResponse> Handle(GetAttendanceRegularizationBySearch request, CancellationToken cancellationToken)
    {
        var query = context.TblAttendanceRegularizations.AsNoTracking().Where(m => m.EmployeeId == request.EmployeeId).AsQueryable();

        if (request.searchParam.FromDate.HasValue)
        {
            query = query.Where(m => m.RegularizationDate >= request.searchParam.FromDate.Value);
        }
        if (request.searchParam.ToDate.HasValue)
        {
            query = query.Where(m => m.RegularizationDate <= request.searchParam.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.searchParam.Status))
        {
            var status = request.searchParam.Status.Trim().ToLowerInvariant();
            query = status switch
            {
                "submitted" => query.Where(m => m.IsSubmit),
                "unsubmitted" => query.Where(m => !m.IsSubmit),
                "approved" => query.Where(m => m.IsApproved),
                "rejected" => query.Where(m => m.IsRejected),
                "pending" => query.Where(m => !m.IsSubmit && !m.IsApproved && !m.IsRejected),
                _ => query
            };
        }

        if (request.searchParam.SortOn is not null && request.searchParam.SortDirection is not null)
        {
            query = request.searchParam.SortOn switch
            {
                _ => request.searchParam.SortDirection == "DESC" ? query.OrderByDescending(m => m.AppliedOn) : query.OrderBy(m => m.AppliedOn),
            };
        }

        var total = await query.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var skip = request.searchParam.PageIndex * request.searchParam.PageSize;

        var data = await query.Skip(skip).Take(request.searchParam.PageSize).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var results = data.Select(m => new RegularizationSearchResult
        {
            Id = m.Id,
            RegularizationDate = m.RegularizationDate,
            Reason = m.Reason,
            AppliedOn = m.AppliedOn,
            Status = GetStatus(m.IsSubmit, m.IsApproved, m.IsRejected),
            AttachmentUrl = m.Attachment
        }).ToList();

        return new GetAttendanceRegularizationBySearchResponse(results, total);
    }

    private static string GetStatus(bool isSubmit, bool isApproved, bool isRejected)
    {
        if (isApproved) return "Approved";
        if (isRejected) return "Rejected";
        if (isSubmit) return "Submitted";
        return "Pending";
    }
}
#endregion
