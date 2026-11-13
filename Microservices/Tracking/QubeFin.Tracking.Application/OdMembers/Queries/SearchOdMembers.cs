using System.Text.Json;
using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.OdMembers.Queries;

#region --- QUERY ---
// SortOn: memberName | overdueAmount | dpd (default). SortDirection: asc | desc (default desc).
// SearchText is one box: member name, member code, loan no, group name or Credit Officer name.
// CreditOfficerId: employee id of the Credit Officer holding the group's designation.
// OnlyUnassigned: only members without a pending OD follow-up task.
public class OdMemberSearchRequest : SearchParam
{
    public Guid? OrganisationUnitId { get; set; }
    public Guid? CreditOfficerId { get; set; }
    public Guid? GroupId { get; set; }
    public DpdRange? DpdRange { get; set; }
    public bool OnlyUnassigned { get; set; }
}

public record SearchOdMembersQuery(Guid UserId, OdMemberSearchRequest Request) : IRequest<Result<OdMemberSearchResponse>>;
#endregion

#region --- VALIDATOR ---
public class SearchOdMembersQueryValidator : AbstractValidator<SearchOdMembersQuery>
{
    private static readonly string[] SortFields = ["memberName", "overdueAmount", "dpd"];
    private static readonly string[] Directions = ["asc", "desc"];

    public SearchOdMembersQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index cannot be negative.");
        RuleFor(v => v.Request.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        RuleFor(v => v.Request.SortOn)
            .Must(s => string.IsNullOrEmpty(s) || SortFields.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort on must be memberName, overdueAmount or dpd.");
        RuleFor(v => v.Request.SortDirection)
            .Must(s => string.IsNullOrEmpty(s) || Directions.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort direction must be asc or desc.");
    }
}
#endregion

#region --- RESPONSE ---
public record OdVisitOutcomeResponse(bool MemberAvailable, bool Paid, decimal? AmountCollected, DateOnly? PtpDate);

public record OdFollowUpResponse(
    Guid JobId,
    string JobCode,
    JobStatus JobStatus,
    Guid TaskId,
    string TaskNo,
    Guid AssigneeId,
    string AssigneeName,
    string? AssigneeRole,
    DateOnly? CompleteBy,
    JobTaskStatus Status,
    OdVisitOutcomeResponse? Outcome);

public record OdPendingFollowUpResponse(Guid JobId, string JobCode, Guid TaskId, string TaskNo, Guid AssigneeId, string AssigneeName);

public record OdMemberResponse(
    Guid Id,
    Guid LoanId,
    string LoanAccountNo,
    string MemberName,
    string MemberCode,
    string? CoBorrowerName,
    Guid GroupId,
    string GroupName,
    string GroupCode,
    Guid? CreditOfficerId,
    string? CreditOfficerName,
    Guid OrganisationUnitId,
    string BranchName,
    string? Phone,
    string? Address,
    decimal OverdueAmount,
    int Dpd,
    DateOnly? LastPaymentDate,
    GeoPoint? HomeLocation,
    bool LocationVerified,
    OdFollowUpResponse? FollowUp,
    List<OdPendingFollowUpResponse> PendingWith);

public record OdMemberSearchResponse(List<OdMemberResponse> Members, int TotalRecords, decimal TotalOverdueAmount);
#endregion

#region --- HANDLER ---
// Overdue members (IsOverdueMember) with their open loan, in the branches of Global.Fn_GetUserWiseBranches.
// Every geo-tagged home counts as verified (there is no verification flag).
internal sealed class SearchOdMembersQueryHandler(
    ITrackingScopeRepository scopeRepository,
    IOdMemberRepository odMemberRepository) : IRequestHandler<SearchOdMembersQuery, Result<OdMemberSearchResponse>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Result<OdMemberSearchResponse>> Handle(SearchOdMembersQuery query, CancellationToken cancellationToken)
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
        var result = await odMemberRepository.SearchAsync(new OdMemberFilter(
            viewer.UserId,
            request.SearchText,
            request.OrganisationUnitId,
            request.CreditOfficerId,
            request.GroupId,
            dpdMin,
            dpdMax,
            request.OnlyUnassigned,
            string.IsNullOrEmpty(request.SortOn) ? null : request.SortOn,
            !string.Equals(request.SortDirection, "asc", StringComparison.OrdinalIgnoreCase),
            request.PageIndex,
            request.PageSize), TrackingClock.Today(), cancellationToken);

        var members = result.Members.Select(r =>
        {
            var home = GeoCoordinates.ToGeoPoint(r.Location);
            return new OdMemberResponse(
                r.Id,
                r.LoanId,
                r.LoanAccountNo,
                r.MemberName,
                r.MemberCode,
                null,
                r.GroupId,
                r.GroupName,
                r.GroupCode,
                r.CreditOfficerId,
                r.CreditOfficerName,
                r.OrganisationUnitId,
                r.BranchName,
                r.Phone,
                string.IsNullOrEmpty(r.Address) ? null : r.Address,
                r.OverdueAmount ?? 0,
                r.Dpd ?? 0,
                r.LastPaymentDate,
                home,
                home is not null,
                FollowUpOf(r.FollowUpJson),
                PendingWithOf(r.PendingWithJson));
        }).ToList();

        return Result.Ok(new OdMemberSearchResponse(members, result.TotalRecords, result.TotalOverdueAmount));
    }

    private sealed record FollowUpRow(
        Guid JobId,
        string JobCode,
        string JobStatus,
        Guid TaskId,
        string TaskNo,
        Guid AssigneeId,
        string AssigneeName,
        string? AssigneeRole,
        DateOnly? CompleteBy,
        string Status,
        bool? MemberAvailable,
        bool? Paid,
        decimal? AmountCollected,
        DateOnly? PtpDate);

    private static OdFollowUpResponse? FollowUpOf(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        var f = JsonSerializer.Deserialize<FollowUpRow>(json, JsonOptions);
        if (f is null) return null;

        var status = Enum.TryParse<JobTaskStatus>(f.Status, true, out var s) ? s : JobTaskStatus.Pending;
        var outcome = status == JobTaskStatus.Completed
            ? new OdVisitOutcomeResponse(f.MemberAvailable ?? false, f.Paid ?? false, f.AmountCollected, f.PtpDate)
            : null;
        return new OdFollowUpResponse(
            f.JobId,
            f.JobCode,
            Enum.TryParse<JobStatus>(f.JobStatus, true, out var js) ? js : JobStatus.Assigned,
            f.TaskId,
            f.TaskNo,
            f.AssigneeId,
            f.AssigneeName,
            f.AssigneeRole,
            f.CompleteBy,
            status,
            outcome);
    }

    private static List<OdPendingFollowUpResponse> PendingWithOf(string? json) =>
        string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<OdPendingFollowUpResponse>>(json, JsonOptions) ?? [];
}
#endregion
