using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.GeoTags.Queries;

#region --- QUERY ---
// Everything comes in the body. SortOn: name (default) | code | branchName | taggedAt. SortDirection: asc (default) | desc.
// PageIndex is 0-based. SearchText matches the name or code.
public class GeoTagSearchRequest : SearchParam
{
    public GeoTagKind? Kind { get; set; }
    public Guid? OrganizationUnitId { get; set; }
    public bool OnlyUntagged { get; set; }
}

public record GetGeoTagsQuery(Guid UserId, GeoTagSearchRequest Request) : IRequest<Result<GeoTagSearchResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGeoTagsQueryValidator : AbstractValidator<GetGeoTagsQuery>
{
    private static readonly string[] SortFields = ["name", "code", "branchName", "taggedAt"];
    private static readonly string[] Directions = ["asc", "desc"];

    public GetGeoTagsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request.Kind).NotNull().WithMessage("Select the kind of place.");
        RuleFor(v => v.Request.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index cannot be negative.");
        RuleFor(v => v.Request.PageSize).InclusiveBetween(1, 500).WithMessage("Page size must be between 1 and 500.");
        RuleFor(v => v.Request.SortOn)
            .Must(s => string.IsNullOrEmpty(s) || SortFields.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort on must be name, code, branchName or taggedAt.");
        RuleFor(v => v.Request.SortDirection)
            .Must(s => string.IsNullOrEmpty(s) || Directions.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort direction must be asc or desc.");
    }
}
#endregion

#region --- RESPONSE ---
public record GeoTagResponse(
    GeoTagKind Kind,
    Guid RefId,
    string Name,
    string? RefCode,
    Guid? BranchId,
    string? BranchName,
    string? Address,
    GeoPoint? Location,
    int? RadiusMeters,
    string? PhotoUrl,
    string? TaggedByName,
    DateTimeOffset? TaggedAt)
{
    // PhotoUrl holds the stored S3 key; it is sent as a signed link
    internal static GeoTagResponse From(GeoTagRow row, IFileStorageRepository fileStorage) => new(
        Enum.Parse<GeoTagKind>(row.Kind),
        row.RefId,
        row.Name,
        row.RefCode,
        row.BranchId,
        row.BranchName,
        row.Address,
        row.Location,
        row.RadiusMeters,
        fileStorage.GetFileUrl(row.PhotoUrl),
        row.TaggedByName,
        TrackingClock.WithIndiaOffset(row.TaggedAt));
}

public record GeoTagSearchResponse(List<GeoTagResponse> Items, int TotalRecords);
#endregion

#region --- HANDLER ---
// Places of one kind with and without a location, in the user's branches.
// Employee homes: super admin sees every active employee; a Divisional, Area or Branch Manager sees the active employees
// posted (Tbl_Employee.OrganizationUnitId) at their unit or any unit under it — DM: the division, its areas and branches;
// AM: the area and its branches; BM: their branch. Nobody else sees employee homes.
internal sealed class GetGeoTagsQueryHandler(ITrackingScopeRepository scopeRepository, IGeoTagRepository geoTagRepository, IFileStorageRepository fileStorage) : IRequestHandler<GetGeoTagsQuery, Result<GeoTagSearchResponse>>
{
    private static readonly string[] EmployeeHomeRoles = [TrackingRole.DivisionalManager, TrackingRole.AreaManager, TrackingRole.BranchManager];

    public async Task<Result<GeoTagSearchResponse>> Handle(GetGeoTagsQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;
        var viewer = await scopeRepository.GetViewerAsync(query.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var kind = request.Kind!.Value;
        var isAdmin = viewer.Role == TrackingRole.Admin;
        var isEmployeeHome = kind == GeoTagKind.EmployeeHome;
        if (isEmployeeHome && !isAdmin && !EmployeeHomeRoles.Contains(viewer.Role))
        {
            return new ForbiddenError("Only a super admin, Divisional Manager, Area Manager or Branch Manager can see employee homes.");
        }

        // Employees can be posted at an area or division office too, so their scope is every unit at or under the user's unit;
        // the other kinds belong to branches
        var unitsInScope = isEmployeeHome ? viewer.UnitIdsInScope : viewer.BranchIds;
        if (request.OrganizationUnitId.HasValue && !isAdmin && !unitsInScope.Contains(request.OrganizationUnitId.Value))
        {
            return new RecordNotFoundError("Branch not found");
        }

        var setting = await scopeRepository.GetTrackingSettingAsync(cancellationToken);
        var result = await geoTagRepository.GetGeoTagsAsync(new GeoTagFilter(
            kind.ToString(),
            isAdmin ? null : unitsInScope,
            request.OrganizationUnitId,
            request.SearchText,
            request.OnlyUntagged,
            setting is null ? null : setting.BranchOperatingRadiusInKm * 1000,
            setting?.TaskLocationMatchInMeters,
            SortOn: string.IsNullOrEmpty(request.SortOn) ? null : request.SortOn,
            SortDescending: string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase),
            PageIndex: request.PageIndex,
            PageSize: request.PageSize), cancellationToken);

        return Result.Ok(new GeoTagSearchResponse(
            result.Rows.Select(r => GeoTagResponse.From(r, fileStorage)).ToList(),
            result.TotalRecords));
    }
}
#endregion
