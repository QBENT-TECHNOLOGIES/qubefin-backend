using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidatesQuery(CandidateSearchParam SearchParam, Guid employeeId) : IRequest<Result<GetCandidatesResponse>>;

public record GetCandidatesResponse(IReadOnlyList<CandidateListDto> candidates, int TotalRecords);

internal sealed class GetCandidatesQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetCandidatesQuery, Result<GetCandidatesResponse>>
{
    public async Task<Result<GetCandidatesResponse>> Handle(GetCandidatesQuery request, CancellationToken cancellationToken)
    {
        var search = request.SearchParam;

        var rows = await context.Set<CandidateListResult>()
            .FromSqlRaw(
                "EXEC [Hrms].[USP_GetCandidateList] @EmployeeId, @SearchText, @ApplicationDateFrom, @ApplicationDateTo, @CompanyId, @InterviewDate, @RecommendationStatus, @Status, @SortOn, @SortDirection, @PageIndex, @PageSize",
                SqlParameters.Value("@EmployeeId", request.employeeId),
                SqlParameters.Text("@SearchText", search.SearchText),
                SqlParameters.Date("@ApplicationDateFrom", search.ApplicationDateFrom),
                SqlParameters.Date("@ApplicationDateTo", search.ApplicationDateTo),
                SqlParameters.Value("@CompanyId", search.CompanyId),
                SqlParameters.Date("@InterviewDate", search.InterviewDate),
                SqlParameters.Text("@RecommendationStatus", search.RecommendationStatus),
                SqlParameters.Text("@Status", search.Status),
                SqlParameters.Text("@SortOn", search.SortOn),
                SqlParameters.Value("@SortDirection", string.Equals(search.SortDirection, "ASC", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC"),
                SqlParameters.Value("@PageIndex", Math.Max(search.PageIndex, 0)),
                SqlParameters.Value("@PageSize", search.PageSize <= 0 ? 10 : search.PageSize))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = new List<CandidateListDto>(rows.Count);
        foreach (var row in rows)
        {
            var status = row.Status ?? CandidateInterviewStatus.SchedulePending;
            items.Add(new CandidateListDto
            {
                Id = row.Id,
                FullName = row.FullName ?? string.Empty,
                ReferenceNo = row.ReferenceNo,
                InterviewPost = row.InterviewPost,
                CompanyName = row.CompanyName,
                ApplicationDate = row.ApplicationDate,
                InterviewDate = row.InterviewDate,
                InterviewTime = row.InterviewTime,
                RecommendationStatus = row.RecommendationStatus ?? "Pending",
                Status = status,
                CanSchedule = row.CanSchedule,
                Downloads = await BuildDownloadsAsync(status, row, cancellationToken)
            });
        }

        return Result.Ok(new GetCandidatesResponse(items, rows.FirstOrDefault()?.TotalRecords ?? 0));
    }
    private async Task<List<CandidateDownloadFileDto>> BuildDownloadsAsync(string status, CandidateListResult row, CancellationToken cancellationToken)
    {
        var stage = status switch
        {
            CandidateInterviewStatus.Joined => 3,
            CandidateInterviewStatus.JoiningInProgress => 2,
            CandidateInterviewStatus.VerificationInProgress => 1,
            _ => 0
        };

        var downloads = new List<CandidateDownloadFileDto>();

        async Task AddFileAsync(string name, string? key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                downloads.Add(new CandidateDownloadFileDto { Name = name, Url = await fileStorageRepository.GetFileUrlAsync(key, cancellationToken) });
            }
        }

        await AddFileAsync("CV", row.CvFile);
        await AddFileAsync("Job Application", row.JobApplicationFile);
        await AddFileAsync("Written Interview Form", row.WrittenInterviewFile);

        if (stage >= 1 && !string.IsNullOrWhiteSpace(row.CreditBureauReportLink))
        {
            downloads.Add(new CandidateDownloadFileDto { Name = "Credit Bureau Report", Url = row.CreditBureauReportLink });
        }

        if (stage >= 2)
        {
            await AddFileAsync("Signed Joining Letter", row.SignedJoiningLetterFile);
        }

        return downloads;
    }
}
