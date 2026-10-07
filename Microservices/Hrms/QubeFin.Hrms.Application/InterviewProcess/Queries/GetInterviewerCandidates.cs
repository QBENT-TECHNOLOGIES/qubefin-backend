using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

/// <summary>Filters of the Interview page list.</summary>
public class InterviewerCandidateSearchParam
{
    /// <summary>Candidate name or reference no. Searching also brings back completed assessments.</summary>
    public string? SearchText { get; set; }
    public string? Status { get; set; }
    public DateOnly? InterviewDate { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public record InterviewerCandidateDto(
    Guid PanelId,
    Guid CandidateId,
    string FullName,
    string? ReferenceNo,
    string? InterviewPost,
    string? CompanyName,
    DateOnly? ApplicationDate,
    DateOnly? InterviewDate,
    TimeOnly? InterviewTime,
    string Status,
    bool IsCandidateAbsent,
    string? AttenedRemarks,
    bool CanAcknowledge,
    bool CanStartAssessment,
    bool CanContinueAssessment,
    bool CanViewAssessment,
    string? CvFileUrl);

/// <summary>Counts behind the Interview page's quick tabs. They follow the search / date filters, not the tab.</summary>
public record InterviewerTabCounts(int AllOpen, int Today, int AcknowledgementPending, int Completed);

public record GetInterviewerCandidatesResponse(IReadOnlyList<InterviewerCandidateDto> Interviews, int TotalRecords, InterviewerTabCounts Counts);

/// <summary>Statuses of the Interview page, decided by Hrms.USP_GetInterviewerCandidateList.</summary>
public static class InterviewerInterviewStatus
{
    public const string AcknowledgementPending = "Acknowledgement Pending";
    public const string Acknowledged = "Acknowledged";
    public const string StartedAssessment = "Started Assessment";
    public const string AssessmentCompleted = "Assessment Completed";
    public const string InterviewClosed = "Interview Closed";

    /// <summary>Tab filter, not a row status: today's interviews that are not completed yet.</summary>
    public const string Today = "Today";
}

/// <summary>The Interview page list - only the interviews the calling employee sits on the panel for. EmployeeId
/// always comes from the caller's claims.</summary>
public record GetInterviewerCandidatesQuery(InterviewerCandidateSearchParam SearchParam, Guid EmployeeId) : IRequest<Result<GetInterviewerCandidatesResponse>>;

internal sealed class GetInterviewerCandidatesQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository)
    : IRequestHandler<GetInterviewerCandidatesQuery, Result<GetInterviewerCandidatesResponse>>
{
    public async Task<Result<GetInterviewerCandidatesResponse>> Handle(GetInterviewerCandidatesQuery request, CancellationToken cancellationToken)
    {
        if (request.EmployeeId == Guid.Empty)
        {
            return Result.Ok(new GetInterviewerCandidatesResponse([], 0, new InterviewerTabCounts(0, 0, 0, 0)));
        }

        var search = request.SearchParam;

        var rows = await context.Set<InterviewerCandidateListResult>()
            .FromSqlRaw(
                "EXEC [Hrms].[USP_GetInterviewerCandidateList] @EmployeeId, @SearchText, @Status, @InterviewDate, @PageIndex, @PageSize",
                SqlParameters.Value("@EmployeeId", request.EmployeeId),
                SqlParameters.Text("@SearchText", search.SearchText),
                SqlParameters.Text("@Status", search.Status),
                SqlParameters.Date("@InterviewDate", search.InterviewDate),
                SqlParameters.Value("@PageIndex", Math.Max(search.PageIndex, 0)),
                SqlParameters.Value("@PageSize", search.PageSize <= 0 ? 10 : search.PageSize))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = new List<InterviewerCandidateDto>(rows.Count);
        foreach (var row in rows)
        {
            items.Add(new InterviewerCandidateDto(
                row.PanelId,
                row.CandidateId,
                row.FullName ?? string.Empty,
                row.ReferenceNo,
                row.InterviewPost,
                row.CompanyName,
                row.ApplicationDate,
                row.InterviewDate,
                row.InterviewTime,
                row.Status ?? InterviewerInterviewStatus.AcknowledgementPending,
                row.IsCandidateAbsent,
                row.AttenedRemarks,
                row.CanAcknowledge,
                row.CanStartAssessment,
                row.CanContinueAssessment,
                row.CanViewAssessment,
                string.IsNullOrWhiteSpace(row.CvFile) ? null : await fileStorageRepository.GetFileUrlAsync(row.CvFile, cancellationToken)));
        }

        var counts = await GetTabCountsAsync(request.EmployeeId, search, cancellationToken);

        return Result.Ok(new GetInterviewerCandidatesResponse(items, rows.FirstOrDefault()?.TotalRecords ?? 0, counts));
    }

    /// <summary>One aggregate over the interviewer's own panel rows. Same rules as the SP's Status column:
    /// completed = submitted or the candidate recorded absent; closed = day passed, rejected or HR completed.</summary>
    private async Task<InterviewerTabCounts> GetTabCountsAsync(Guid employeeId, InterviewerCandidateSearchParam search, CancellationToken cancellationToken)
    {
        var today = CandidateWorkflow.Today;
        var query = context.TblInterviewPanels.AsNoTracking().Where(p => p.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(search.SearchText))
        {
            var term = search.SearchText.Trim();
            query = query.Where(p =>
                (p.Candidate.FirstName + " " + (p.Candidate.MiddleName ?? "") + " " + p.Candidate.LastName).Contains(term) ||
                (p.Candidate.FirstName + " " + p.Candidate.LastName).Contains(term) ||
                (p.Candidate.ReferenceNo != null && p.Candidate.ReferenceNo.Contains(term)));
        }

        if (search.InterviewDate is { } interviewDate)
        {
            query = query.Where(p => p.Candidate.InterviewDate == interviewDate);
        }

        var counts = await query
            .Select(p => new
            {
                IsCompleted = p.IsSubmitted || (!p.IsAttened && p.AttenedRemarks != null),
                IsToday = p.Candidate.InterviewDate == today,
                IsClosed = p.Candidate.InterviewDate == null || p.Candidate.InterviewDate < today ||
                           p.Candidate.RecommendationStatus == CandidateWorkflow.Rejected || p.Candidate.IsHrAssessmentCompleted,
                p.IsAcknowledged
            })
            .GroupBy(_ => 1)
            .Select(g => new InterviewerTabCounts(
                g.Count(x => !x.IsCompleted),
                g.Count(x => !x.IsCompleted && x.IsToday),
                g.Count(x => !x.IsCompleted && !x.IsClosed && !x.IsAcknowledged),
                g.Count(x => x.IsCompleted)))
            .FirstOrDefaultAsync(cancellationToken);

        return counts ?? new InterviewerTabCounts(0, 0, 0, 0);
    }
}
