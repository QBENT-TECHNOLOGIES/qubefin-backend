using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using System.Globalization;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidatesQuery(CandidateSearchParam SearchParam, Guid employeeId) : IRequest<Result<GetCandidatesResponse>>;

public record GetCandidatesResponse(IReadOnlyList<CandidateListDto> candidates, int TotalRecords);


internal sealed class GetCandidatesQueryHandler(QubeFinDataContext context, IConfiguration configuration, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetCandidatesQuery, Result<GetCandidatesResponse>>
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
            .Include(c => c.CreatedByNavigation)
            .Include(m => m.InterviewPostNavigation)
            .Include(m => m.TblInterviewPanels)
            .Include(e => e.TblEmployees)
            .AsNoTracking();

        // Access filter
        if (!isHrEmployee)
        {
            query = query.Where(c =>
                // Employee who created the candidate
                c.CreatedByNavigation.EmployeeId == request.employeeId

                ||

                // Employee is part of the interview panel
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
        var rows = await query
        .Skip(pageIndex * pageSize)
        .Take(pageSize)
        .Select(c => new
        {
            c.WrittenInterviewFile,
            c.SignedJoiningLetterFile,
            c.CreditBureauReportLink,
            Candidate = new CandidateListDto
            {
                Id = c.Id,
                FullName = ((c.FirstName + " " + (c.MiddleName ?? string.Empty) + " " + c.LastName).Replace("  ", " ").Trim()),
                InterviewPost = c.InterviewPostNavigation.Name,
                InterviewDate = c.InterviewDate,
                InterviewTime = c.InterviewTime != null ? c.InterviewTime.Value.ToString("h.mm tt", CultureInfo.InvariantCulture).ToLowerInvariant() : string.Empty,
                RecommendationStatus = c.RecommendationStatus ?? "Pending",
                ReferenceNo = c.ReferenceNo,
                // HR assessment counts as submitted once RecommendationStatus leaves 'Pending' - the same signal
                // the HR Assessment form and USP_GetInterviewCandidateById use.
                InterviewStatus = c.SignedJoiningLetterFile != null && c.SignedJoiningLetterFile != "" && c.TblEmployees.Any()
                    ? CandidateInterviewStatus.Joined
                    : c.IsOfferLetterReceived
                        ? CandidateInterviewStatus.JoiningInProgress
                        : c.RecommendationStatus != null && c.RecommendationStatus != "" && c.RecommendationStatus != "Pending"
                            ? CandidateInterviewStatus.VerificationInProgress
                            : CandidateInterviewStatus.InterviewInProgress
            }
        })
        .ToListAsync(cancellationToken);

        var items = new List<CandidateListDto>(rows.Count);
        foreach (var row in rows)
        {
            var candidate = row.Candidate;
            candidate.Downloads = await BuildDownloadsAsync(candidate.InterviewStatus, row.WrittenInterviewFile, row.CreditBureauReportLink, row.SignedJoiningLetterFile, cancellationToken);
            items.Add(candidate);
        }

        return Result.Ok(new GetCandidatesResponse(items, totalCount));
    }

    /// <summary>Each stage unlocks the files produced in it, and keeps the earlier stages' files available:
    /// the written interview form from the interview, the credit bureau report once verification starts and
    /// the signed joining letter once joining starts. A file that was never uploaded is left out.</summary>
    private async Task<List<CandidateDownloadFileDto>> BuildDownloadsAsync(string interviewStatus, string? writtenInterviewFile, string? creditBureauReportLink, string? signedJoiningLetterFile, CancellationToken cancellationToken)
    {
        var stage = interviewStatus switch
        {
            CandidateInterviewStatus.Joined => 3,
            CandidateInterviewStatus.JoiningInProgress => 2,
            CandidateInterviewStatus.VerificationInProgress => 1,
            _ => 0
        };

        var downloads = new List<CandidateDownloadFileDto>();

        if (!string.IsNullOrWhiteSpace(writtenInterviewFile))
        {
            downloads.Add(new CandidateDownloadFileDto { Name = "Written Interview Form", Url = await fileStorageRepository.GetFileUrlAsync(writtenInterviewFile, cancellationToken) });
        }

        // Entered as a link on the verification form, not uploaded to storage.
        if (stage >= 1 && !string.IsNullOrWhiteSpace(creditBureauReportLink))
        {
            downloads.Add(new CandidateDownloadFileDto { Name = "Credit Bureau Report", Url = creditBureauReportLink });
        }

        if (stage >= 2 && !string.IsNullOrWhiteSpace(signedJoiningLetterFile))
        {
            downloads.Add(new CandidateDownloadFileDto { Name = "Signed Joining Letter", Url = await fileStorageRepository.GetFileUrlAsync(signedJoiningLetterFile, cancellationToken) });
        }

        return downloads;
    }
}
