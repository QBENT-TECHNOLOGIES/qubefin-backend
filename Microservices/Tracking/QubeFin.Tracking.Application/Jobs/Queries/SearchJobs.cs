using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Queries;

#region --- QUERY ---
public record SearchJobsQuery(Guid UserId, JobSearchRequest Request) : IRequest<Result<JobSearchResponse>>;
#endregion

#region --- VALIDATOR ---
public class SearchJobsQueryValidator : AbstractValidator<SearchJobsQuery>
{
    private static readonly string[] SortFields = ["endDate", "completeBy", "createdOn", "createdAt", "assigneeName", "status"];

    public SearchJobsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index cannot be negative.");
        RuleFor(v => v.Request.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        RuleFor(v => v.Request.SortOn)
            .Must(s => s is null || SortFields.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort on must be endDate, createdOn, assigneeName or status.");
    }
}
#endregion

#region --- RESPONSE ---
public record JobSummaryResponse(
    Guid Id,
    string JobCode,
    string JobTitle,
    JobStatus Status,
    Guid? AssigneeId,
    string? AssigneeName,
    string AssigneeRole,
    string UnitName,
    IReadOnlyList<Guid> OrganisationUnitIds,
    IReadOnlyList<string> BranchNames,
    string CreatedByName,
    DateTime? CreatedOn,
    DateOnly StartDate,
    DateOnly? EndDate,
    int TaskCount,
    int DoneCount,
    int OpenCount,
    int SelfAddedCount,
    IReadOnlyList<TaskType> Types,
    Guid? TransferJobRefId,
    string? TransferJobRefCode);

public record JobSearchResponse(List<JobSummaryResponse> Jobs, int TotalRecords, JobStatusCounts Summary);
#endregion

#region --- HANDLER ---
internal sealed class SearchJobsQueryHandler(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository) : IRequestHandler<SearchJobsQuery, Result<JobSearchResponse>>
{
    private static readonly TimeZoneInfo IndiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

    public async Task<Result<JobSearchResponse>> Handle(SearchJobsQuery query, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(query.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var request = query.Request;
        var (dpdMin, dpdMax) = request.DpdRange switch
        {
            DpdRange.Days1To30 => ((int?)1, (int?)30),
            DpdRange.Days31To60 => (31, 60),
            DpdRange.Days61To90 => (61, 90),
            DpdRange.Over90 => (91, (int?)null),
            _ => ((int?)null, (int?)null)
        };
        var filter = new JobSearchFilter(
            request.SearchText,
            request.Status?.ToString(),
            request.OpenOnly,
            request.OrganisationUnitId,
            request.AssigneeId,
            request.DueFrom,
            request.DueTo,
            dpdMin,
            dpdMax,
            request.PageIndex,
            request.PageSize,
            request.SortOn,
            request.SortDirection == SortDirection.Desc);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaTimeZone));

        var result = await jobRepository.SearchJobsAsync(viewer, filter, today, cancellationToken);

        var jobs = result.Jobs.Select(j => new JobSummaryResponse(
            j.Id,
            j.JobCode,
            j.JobTitle,
            Enum.Parse<JobStatus>(j.Status),
            j.AssigneeId,
            j.AssigneeName,
            j.AssigneeRole,
            j.UnitName,
            j.OrganisationUnitIds,
            j.BranchNames,
            j.CreatedByName,
            j.CreatedOn,
            j.StartDate,
            j.EndDate,
            j.TaskCount,
            j.DoneCount,
            j.OpenCount,
            j.SelfAddedCount,
            j.Types.Select(t => Enum.TryParse<TaskType>(t, true, out var type) ? (TaskType?)type : null).OfType<TaskType>().ToList(),
            j.TransferJobRefId,
            j.TransferJobRefCode)).ToList();

        return Result.Ok(new JobSearchResponse(jobs, result.TotalRecords, result.Summary));
    }
}
#endregion
