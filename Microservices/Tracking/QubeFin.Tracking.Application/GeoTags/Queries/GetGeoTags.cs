using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.GeoTags.Queries;

#region --- QUERY ---
public record GetGeoTagsQuery(Guid UserId, GeoTagKind? Kind, string? SearchText, Guid? OrganisationUnitId, bool OnlyUntagged) : IRequest<Result<List<GeoTagResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetGeoTagsQueryValidator : AbstractValidator<GetGeoTagsQuery>
{
    public GetGeoTagsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Kind).NotNull().WithMessage("Select the kind of place.");
    }
}
#endregion

#region --- RESPONSE ---
public record GeoTagChangeResponse(GeoPoint Location, int RadiusMeters, string ChangedByName, DateTimeOffset ChangedAt, string? Reason);

public record GeoTagResponse(
    Guid Id,
    GeoTagKind Kind,
    Guid RefId,
    string Name,
    string? RefCode,
    Guid? OrganisationUnitId,
    string? BranchName,
    string? Address,
    GeoPoint? Location,
    int? RadiusMeters,
    string? PhotoUrl,
    bool Verified,
    string? TaggedByName,
    DateTimeOffset? TaggedAt,
    List<GeoTagChangeResponse> History);
#endregion

#region --- HANDLER ---
// Places of one kind with and without a location, in the user's branches. Employee homes are for an Admin only.
// There is no geo-tag history yet, so taggedByName, taggedAt and photoUrl are null, history is empty and every place counts as verified.
internal sealed class GetGeoTagsQueryHandler(ITrackingScopeRepository scopeRepository, IGeoTagRepository geoTagRepository) : IRequestHandler<GetGeoTagsQuery, Result<List<GeoTagResponse>>>
{
    public async Task<Result<List<GeoTagResponse>>> Handle(GetGeoTagsQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var kind = request.Kind!.Value;
        var isAdmin = viewer.Role == TrackingRole.Admin;
        if (kind == GeoTagKind.EmployeeHome && !isAdmin) return new ForbiddenError("Only an Admin can see employee homes.");
        if (request.OrganisationUnitId.HasValue && !isAdmin && !viewer.BranchIds.Contains(request.OrganisationUnitId.Value))
        {
            return new RecordNotFoundError("Branch not found");
        }

        var setting = await scopeRepository.GetTrackingSettingAsync(cancellationToken);
        var rows = await geoTagRepository.GetGeoTagsAsync(new GeoTagFilter(
            kind.ToString(),
            isAdmin ? null : viewer.BranchIds,
            request.OrganisationUnitId,
            request.SearchText,
            request.OnlyUntagged,
            setting is null ? null : setting.BranchOperatingRadiusInKm * 1000,
            setting?.TaskLocationMatchInMeters), cancellationToken);

        return Result.Ok(rows.Select(r => new GeoTagResponse(
            r.Id,
            kind,
            r.RefId,
            r.Name,
            r.RefCode,
            r.OrganisationUnitId,
            r.BranchName,
            r.Address,
            r.Location,
            r.RadiusMeters,
            null,
            true,
            null,
            null,
            [])).ToList());
    }
}
#endregion
