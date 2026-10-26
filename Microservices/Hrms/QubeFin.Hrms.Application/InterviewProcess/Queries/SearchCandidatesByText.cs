using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record SearchCandidatesByTextQuery(string SearchText, int MaxResults = 50) : IRequest<Result<List<SearchCandidatesByTextResponse>>>;

public record SearchCandidatesByTextResponse(Guid Id, string DisplayName);


internal sealed class SearchCandidatesByTextQueryHandler(QubeFinDataContext context) : IRequestHandler<SearchCandidatesByTextQuery, Result<List<SearchCandidatesByTextResponse>>>
{
    public async Task<Result<List<SearchCandidatesByTextResponse>>> Handle(SearchCandidatesByTextQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchText))
        {
            return Result.Ok(new List<SearchCandidatesByTextResponse> ());
        }

        var term = request.SearchText.Trim();

        var query = context.TblInterviewCandidates
            .AsNoTracking()
            .Where(c =>
                c.FirstName.Contains(term) ||
                c.LastName.Contains(term) ||
                c.MobileNo.Contains(term) ||
                (c.Email != null && c.Email.Contains(term)) ||
                (c.ReferenceNo != null && c.ReferenceNo.Contains(term)));

        var items = await query
            .OrderByDescending(c => c.CreatedOn)
            .Take(request.MaxResults)
            .Select(c => new SearchCandidatesByTextResponse(c.Id, ((c.FirstName + " " + (c.MiddleName ?? string.Empty) + " " + c.LastName)
                                .Replace("  ", " ").Trim()
                               + (string.IsNullOrWhiteSpace(c.ReferenceNo) ? string.Empty : " (" + c.ReferenceNo + ")")))).ToListAsync(cancellationToken);

        return Result.Ok(items);
    }
}
