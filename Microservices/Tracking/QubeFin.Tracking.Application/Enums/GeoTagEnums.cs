namespace QubeFin.Tracking.Application.Enums;

// Values match the web models (features/tracking/models/geo-tag.ts) and are sent as plain strings.

// Member and OdMember are the member's home
public enum GeoTagKind
{
    Branch,
    GroupCentre,
    Member,
    OdMember,
    EmployeeHome
}
