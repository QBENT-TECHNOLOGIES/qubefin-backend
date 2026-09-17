using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidatesQuery(CandidateSearchParam SearchParam) : IRequest<Result<GetCandidatesResponse>>;

public record GetCandidatesResponse(IReadOnlyList<CandidateListDto> candidates, int TotalRecords);


internal sealed class GetCandidatesQueryHandler(QubeFinDataContext context) : IRequestHandler<GetCandidatesQuery, Result<GetCandidatesResponse>>
{
    public async Task<Result<GetCandidatesResponse>> Handle(GetCandidatesQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.SearchParam.PageIndex < 0 ? 0 : request.SearchParam.PageIndex;
        var pageSize = request.SearchParam.PageSize <= 0 ? 10 : request.SearchParam.PageSize;
        var query = context.TblInterviewCandidates.Include(m => m.InterviewPost)
            .AsNoTracking();
            //.Where(c => c.CompanyId == request.SearchParam.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.SearchParam.SearchText))
        {
            var term = request.SearchParam.SearchText.Trim();
            query = query.Where(c =>
                c.FirstName.Contains(term) ||
                c.LastName.Contains(term) ||
                c.MobileNo.Contains(term) ||
                (c.Email != null && c.Email.Contains(term)) ||
                (c.ReferenceNo != null && c.ReferenceNo.Contains(term)));
        }

        query = request.SearchParam.SortOn switch
        {
            "name" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase)
                ? query.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.LastName)
                : query.OrderBy(c => c.FirstName).ThenBy(c => c.LastName),
            "interviewDate" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase)
                ? query.OrderByDescending(c => c.InterviewDate)
                : query.OrderBy(c => c.InterviewDate),
            _ => query.OrderByDescending(c => c.CreatedOn)
        };

        var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(c => new CandidateListDto
            {
                Id = c.Id,
                    FullName = ((c.FirstName + " " + (c.MiddleName ?? string.Empty) + " " + c.LastName).Replace("  ", " ").Trim() 
                                + (string.IsNullOrWhiteSpace(c.ReferenceNo) ? string.Empty : " (" + c.ReferenceNo + ")")),
                InterviewPost = c.InterviewPost,
                InterviewDate = c.InterviewDate,
                RecommendationStatus = c.RecommendationStatus ?? "Pending",
                TotalRatingPoint = c.TotalRatingPoint,
                RatingStatus = c.RatingStatus ?? "Not started",
                ReferenceNo = c.ReferenceNo
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(new GetCandidatesResponse(items, totalCount));
    }
}
