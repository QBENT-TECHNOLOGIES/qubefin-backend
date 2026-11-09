using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.BcLists.Queries;

#region --- QUERY ---
// SortOn: name | code | branchName | formationDate | createdDate. SearchText matches the group name or code.
public class GroupListRequest : SearchParam
{
    public Guid? OrganisationUnitId { get; set; }
    public Guid? CoEmployeeId { get; set; }
}

public record GetGroupListQuery(Guid UserId, GroupListRequest Request) : IRequest<Result<GroupListResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupListQueryValidator : AbstractValidator<GetGroupListQuery>
{
    public GetGroupListQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index cannot be negative.");
        RuleFor(v => v.Request.PageSize).InclusiveBetween(1, 500).WithMessage("Page size must be between 1 and 500.");
    }
}
#endregion

#region --- RESPONSE ---
public record GroupListItem(
    Guid Id,
    string Code,
    string Name,
    string? AddressLine1,
    Guid? AdministrativeUnitId,
    string? AdministrativeUnitName,
    string? PinCode,
    GeoPoint? Location,
    string? PhotoUrl,
    Guid OrganisationUnitId,
    string BranchName,
    Guid DesignationId,
    Guid? CoEmployeeId,
    string? CoName,
    DateOnly? FormationDate,
    string? ContactNo,
    string? CollectionDay,
    TimeOnly? CollectionTime,
    DateOnly? BiWeeklyStartDate,
    bool IsMonthlyGroup,
    DateTime? CreatedDate,
    Guid? CreatedBy,
    string? CreatedByName,
    Guid? TaggedBy,
    string? TaggedByName,
    DateTimeOffset? TaggedOn);

public record GroupListResponse(List<GroupListItem> Items, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetGroupListQueryHandler(
    ITrackingScopeRepository scopeRepository,
    IBcListRepository listRepository,
    IFileStorageRepository fileStorage) : IRequestHandler<GetGroupListQuery, Result<GroupListResponse>>
{
    public async Task<Result<GroupListResponse>> Handle(GetGroupListQuery query, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(query.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var request = query.Request;
        var rows = await listRepository.GetGroupsAsync(new BcListFilter(
            viewer.UserId,
            request.OrganisationUnitId,
            request.CoEmployeeId,
            null,
            request.SearchText,
            request.SortOn,
            request.SortDirection,
            request.PageIndex,
            request.PageSize), cancellationToken);

        var items = rows.Select(r => new GroupListItem(
            r.Id,
            r.Code,
            r.Name,
            r.AddressLine1,
            r.AdministrativeUnitId,
            r.AdministrativeUnitName,
            r.PinCode,
            ToGeoPoint(r.Latitude, r.Longitude),
            fileStorage.GetFileUrl(r.LocationPhoto),
            r.OrganizationUnitId,
            r.BranchName,
            r.DesignationId,
            r.CoEmployeeId,
            r.CoName,
            r.FormationDate,
            r.ContactNo,
            r.CollectionDay,
            r.CollectionTime,
            r.BiWeeklyStartDate,
            r.IsMonthlyGroup,
            r.CreatedDate,
            r.CreatedBy,
            r.CreatedByName,
            r.TaggedBy,
            r.TaggedByName,
            TrackingClock.WithIndiaOffset(r.TaggedOn))).ToList();

        return Result.Ok(new GroupListResponse(items, rows.FirstOrDefault()?.TotalRecords ?? 0));
    }

    internal static GeoPoint? ToGeoPoint(double? latitude, double? longitude) =>
        latitude.HasValue && longitude.HasValue ? new GeoPoint(Math.Round((decimal)latitude.Value, 6), Math.Round((decimal)longitude.Value, 6)) : null;
}
#endregion
