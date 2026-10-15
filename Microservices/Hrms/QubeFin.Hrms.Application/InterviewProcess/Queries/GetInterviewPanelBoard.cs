using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetInterviewPanelBoardQuery(InterviewPanelBoardSearchParam SearchParam, Guid EmployeeId) : IRequest<Result<List<InterviewPanelSlotDto>>>;

internal sealed class GetInterviewPanelBoardQueryHandler(QubeFinDataContext context, IConfiguration configuration)
    : IRequestHandler<GetInterviewPanelBoardQuery, Result<List<InterviewPanelSlotDto>>>
{
    private const int DefaultRangeInDays = 30;

    public async Task<Result<List<InterviewPanelSlotDto>>> Handle(GetInterviewPanelBoardQuery request, CancellationToken cancellationToken)
    {
        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken))
        {
            return new ForbiddenError("Only HR can manage panel tagging.");
        }

        var today = CandidateWorkflow.Today;
        var from = request.SearchParam.DateFrom ?? today;
        var to = request.SearchParam.DateTo ?? from.AddDays(DefaultRangeInDays);
        var venueId = request.SearchParam.VenueOrganizationUnitId;
        var companyId = request.SearchParam.CompanyId;

        var candidates = await context.TblInterviewCandidates
            .AsNoTracking()
            .Where(c => c.InterviewDate != null && c.InterviewDate >= from && c.InterviewDate <= to
                     && c.InterviewTime != null
                     && c.VenueOrganizationUnitId != null
                     && (c.RecommendationStatus == null || c.RecommendationStatus != CandidateWorkflow.Rejected))
            .Where(c => venueId == null || c.VenueOrganizationUnitId == venueId)
            .Where(c => companyId == null || c.CompanyId == companyId)
            .Select(c => new
            {
                c.Id,
                c.FirstName,
                c.MiddleName,
                c.LastName,
                c.ReferenceNo,
                InterviewPost = c.InterviewPostNavigation.Name,
                InterviewDate = c.InterviewDate!.Value,
                c.InterviewTime,
                VenueId = c.VenueOrganizationUnitId!.Value,
                VenueName = c.VenueOrganizationUnit!.Name,
                c.IsHrAssessmentCompleted,
                Panelists = c.TblInterviewPanels
                    .Select(p => new { p.EmployeeId, p.IsAcknowledged, p.IsSubmitted, IsAttendanceMarked = p.IsAttened || p.AttenedRemarks != null })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var employeeIds = candidates.SelectMany(c => c.Panelists).Select(p => p.EmployeeId).Distinct().ToList();
        var employees = await context.TblEmployees
            .AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .Select(e => new PanelMemberDto(
                e.Id,
                e.FullName,
                e.Code,
                e.TblEmployeeDesignations
                    .Where(d => d.EffectiveTo == null)
                    .OrderByDescending(d => d.EffectiveFrom)
                    .Select(d => d.Designation.Name)
                    .FirstOrDefault()))
            .ToDictionaryAsync(e => e.EmployeeId, cancellationToken);

        var slots = candidates
            .GroupBy(c => new { c.InterviewDate, c.VenueId, c.VenueName })
            .OrderBy(g => g.Key.InterviewDate)
            .ThenBy(g => g.Key.VenueName)
            .Select(slot =>
            {
                var isPast = slot.Key.InterviewDate < today;

                var rows = slot.Select(c =>
                {
                    var lockedReason = c.IsHrAssessmentCompleted ? "HR Assessment is completed."
                        : c.Panelists.Any(p => p.IsSubmitted) ? "A panelist has already submitted the assessment."
                        : c.Panelists.Any(p => p.IsAttendanceMarked) ? "The interview has already started."
                        : isPast ? "The interview date has passed."
                        : null;

                    var dto = new PanelCandidateDto(
                        c.Id,
                        string.Join(" ", new[] { c.FirstName, c.MiddleName, c.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))),
                        c.ReferenceNo,
                        c.InterviewPost,
                        c.InterviewTime,
                        c.Panelists.Count,
                        c.Panelists.Count(p => p.IsAcknowledged),
                        c.Panelists.Count(p => p.IsSubmitted),
                        lockedReason is not null,
                        lockedReason);

                    return new { Dto = dto, PanelKey = PanelKey(c.Panelists.Select(p => p.EmployeeId)), c.Panelists, c.IsHrAssessmentCompleted };
                }).ToList();

                var panels = rows
                    .Where(r => r.Panelists.Count > 0)
                    .GroupBy(r => r.PanelKey)
                    .Select(g => new InterviewPanelGroupDto(
                        g.Key,
                        g.First().Panelists
                            .Select(p => employees.TryGetValue(p.EmployeeId, out var e) ? e : new PanelMemberDto(p.EmployeeId, "Unknown", string.Empty, null))
                            .OrderBy(e => e.EmployeeName)
                            .ToList(),
                        g.Select(r => r.Dto).OrderBy(d => d.InterviewTime).ThenBy(d => d.FullName).ToList()))
                    .OrderBy(p => p.Members.FirstOrDefault()?.EmployeeName)
                    .ToList();

                // Untagged candidates whose HR assessment is already done cannot be tagged any more.
                var untagged = rows
                    .Where(r => r.Panelists.Count == 0 && !r.IsHrAssessmentCompleted)
                    .Select(r => r.Dto)
                    .OrderBy(d => d.InterviewTime).ThenBy(d => d.FullName)
                    .ToList();

                return new InterviewPanelSlotDto(slot.Key.InterviewDate, slot.Key.VenueId, slot.Key.VenueName, isPast, panels, untagged);
            })
            .Where(s => s.Panels.Count > 0 || s.UntaggedCandidates.Count > 0)
            .ToList();

        return Result.Ok(slots);
    }

    /// <summary>Same panelists (in any order) => same key => same panel. Sorted as text so the UI can build the same key.</summary>
    internal static string PanelKey(IEnumerable<Guid> employeeIds) =>
        string.Join(",", employeeIds.Select(id => id.ToString("N")).Order(StringComparer.Ordinal));
}
