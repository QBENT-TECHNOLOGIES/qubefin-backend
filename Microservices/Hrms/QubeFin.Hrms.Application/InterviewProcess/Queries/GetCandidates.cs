using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Persistence;
using System.Globalization;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidatesQuery(CandidateSearchParam SearchParam, Guid employeeId) : IRequest<Result<GetCandidatesResponse>>;

public record GetCandidatesResponse(IReadOnlyList<CandidateListDto> candidates, int TotalRecords);


internal sealed class GetCandidatesQueryHandler(QubeFinDataContext context, IConfiguration configuration) : IRequestHandler<GetCandidatesQuery, Result<GetCandidatesResponse>>
{
    public async Task<Result<GetCandidatesResponse>> Handle(GetCandidatesQuery request, CancellationToken cancellationToken)
    {
        var hrPostId = Guid.Parse(configuration["HrPost"]!);

        // Check whether logged-in employee is HR
        var isHrEmployee = await context.TblDesignations
            .AnyAsync(d =>
                d.PostId == hrPostId &&
                d.TblEmployeeDesignations.Any(ed =>
                    ed.EmployeeId == request.employeeId &&
                    ed.EffectiveTo == null
                )
            );

        var pageIndex = request.SearchParam.PageIndex < 0
            ? 0
            : request.SearchParam.PageIndex;

        var pageSize = request.SearchParam.PageSize <= 0
            ? 10
            : request.SearchParam.PageSize;

        var query = context.TblInterviewCandidates
            .Include(m => m.InterviewPostNavigation)
            .Include(m => m.TblInterviewPanels)
            .AsNoTracking();

        // Access filter
        if (!isHrEmployee)
        {
            query = query.Where(c =>
                c.TblInterviewPanels.Any(p =>
                    p.EmployeeId == request.employeeId
                )
            );
        }

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.SearchParam.SearchText))
        {
            var term = request.SearchParam.SearchText.Trim();

            query = query.Where(c =>
                c.FirstName.Contains(term) ||
                c.LastName.Contains(term) ||
                c.MobileNo.Contains(term) ||
                (c.Email != null && c.Email.Contains(term)) ||
                (c.ReferenceNo != null && c.ReferenceNo.Contains(term))
            );
        }
        // Search filter
        if (request.SearchParam.CompanyId != null)
        {

            query = query.Where(c =>c.CompanyId == request.SearchParam.CompanyId);
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
            FullName = ((c.FirstName + " " + (c.MiddleName ?? string.Empty) + " " + c.LastName).Replace("  ", " ").Trim()),
            InterviewPost = c.InterviewPostNavigation.Name,
            InterviewDate = c.InterviewDate,
            InterviewTime = c.InterviewTime != null ? c.InterviewTime.Value.ToString("h.mm tt", CultureInfo.InvariantCulture).ToLowerInvariant() : string.Empty,
            RecommendationStatus = c.RecommendationStatus ?? "Pending",
            ReferenceNo = c.ReferenceNo
        })
        .ToListAsync(cancellationToken);

        return Result.Ok(new GetCandidatesResponse(items, totalCount));
    }
}
