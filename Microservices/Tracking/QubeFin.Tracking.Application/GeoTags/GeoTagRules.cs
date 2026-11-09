using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.GeoTags;

// Who may set or change a location:
//   group or member: whoever holds the group's mapped designation (the CO), and the Branch, Area or Divisional Manager above it in scope;
//   branch: a Branch, Area or Divisional Manager whose branches include it;
//   employee home: a super admin only (employee tagging is still to be decided).
internal static class GeoTagRules
{
    private static readonly string[] Managers = [TrackingRole.BranchManager, TrackingRole.AreaManager, TrackingRole.DivisionalManager];

    public static bool IsGroupOrMember(GeoTagTarget target) => target.Kind is "GroupCentre" or "Member" or "OdMember";

    public static bool CanEdit(TrackingViewer viewer, GeoTagTarget target) => target.Kind switch
    {
        "Branch" => Managers.Contains(viewer.Role) && viewer.BranchIds.Contains(target.BranchId!.Value),
        "EmployeeHome" => viewer.Role == TrackingRole.Admin,
        _ => target.IsOwnedByViewer
            || (Managers.Contains(viewer.Role)
                && target.OwnerPostName is not null
                && TrackingRole.Rank(viewer.Role) > TrackingRole.Rank(target.OwnerPostName)
                && viewer.UnitIdsInScope.Contains(target.OwnerUnitId!.Value))
    };

    // The place as E1 shows it, read back after a change
    public static async Task<GeoTagRow> ReadBackAsync(ITrackingScopeRepository scopeRepository, IGeoTagRepository geoTagRepository, GeoTagTarget target, CancellationToken cancellationToken)
    {
        var setting = await scopeRepository.GetTrackingSettingAsync(cancellationToken);
        return (await geoTagRepository.GetGeoTagsAsync(new GeoTagFilter(
            target.Kind,
            null,
            null,
            null,
            false,
            setting is null ? null : setting.BranchOperatingRadiusInKm * 1000,
            setting?.TaskLocationMatchInMeters,
            target.RefId), cancellationToken)).Single();
    }
}
