using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.BcLists.Queries;

#region --- QUERY ---
// SortOn: name | code | groupName | branchName | createdDate.
// SearchText is one box: name, member code, Aadhaar / PAN / Voter no or mobile.
public class MemberListRequest : SearchParam
{
    public Guid? OrganisationUnitId { get; set; }
    public Guid? CoEmployeeId { get; set; }
    public Guid? GroupId { get; set; }
}

public record GetMemberListQuery(Guid UserId, MemberListRequest Request) : IRequest<Result<MemberListResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetMemberListQueryValidator : AbstractValidator<GetMemberListQuery>
{
    public GetMemberListQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index cannot be negative.");
        RuleFor(v => v.Request.PageSize).InclusiveBetween(1, 500).WithMessage("Page size must be between 1 and 500.");
    }
}
#endregion

#region --- RESPONSE ---
public record MemberListItem(
    Guid Id,
    string Code,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? FatherName,
    DateOnly? DateOfBirth,
    string? Gender,
    string? MaritalStatus,
    string? Religion,
    string? Caste,
    string? Qualification,
    string? AadharNo,
    string? Pan,
    string? VoterNo,
    string? MobileNo,
    string? AddressLine1,
    string? AddressLine2,
    string? PinCode,
    GeoPoint? Location,
    string? PhotoUrl,
    Guid GroupId,
    string GroupCode,
    string GroupName,
    Guid OrganisationUnitId,
    string BranchName,
    Guid? CoEmployeeId,
    string? CoName,
    bool IsOverdueMember,
    DateTime? CreatedDate,
    Guid? CreatedBy,
    string? CreatedByName,
    Guid? TaggedBy,
    string? TaggedByName,
    DateTimeOffset? TaggedOn);

public record MemberListResponse(List<MemberListItem> Items, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetMemberListQueryHandler(
    ITrackingScopeRepository scopeRepository,
    IBcListRepository listRepository,
    IFileStorageRepository fileStorage) : IRequestHandler<GetMemberListQuery, Result<MemberListResponse>>
{
    public async Task<Result<MemberListResponse>> Handle(GetMemberListQuery query, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(query.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var request = query.Request;
        var rows = await listRepository.GetMembersAsync(new BcListFilter(
            viewer.UserId,
            request.OrganisationUnitId,
            request.CoEmployeeId,
            request.GroupId,
            request.SearchText,
            request.SortOn,
            request.SortDirection,
            request.PageIndex,
            request.PageSize), cancellationToken);

        var items = rows.Select(r => new MemberListItem(
            r.Id,
            r.Code,
            r.FirstName,
            r.MiddleName,
            r.LastName,
            r.FatherName,
            r.DateOfBirth,
            r.Gender,
            r.MaritalStatus,
            r.Religion,
            r.Caste,
            r.Qualification,
            r.AadharNo,
            r.Pan,
            r.VoterNo,
            r.MobileNo,
            r.AddressLine1,
            r.AddressLine2,
            r.PinCode,
            GetGroupListQueryHandler.ToGeoPoint(r.Latitude, r.Longitude),
            fileStorage.GetFileUrl(r.LocationPhoto),
            r.GroupId,
            r.GroupCode,
            r.GroupName,
            r.OrganizationUnitId,
            r.BranchName,
            r.CoEmployeeId,
            r.CoName,
            r.IsOverdueMember,
            r.CreatedDate,
            r.CreatedBy,
            r.CreatedByName,
            r.TaggedBy,
            r.TaggedByName,
            TrackingClock.WithIndiaOffset(r.TaggedOn))).ToList();

        return Result.Ok(new MemberListResponse(items, rows.FirstOrDefault()?.TotalRecords ?? 0));
    }
}
#endregion
