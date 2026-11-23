namespace QubeFin.Persistence.Models.Tracking;

// Role names are Tbl_Post.Name, except Admin (Tbl_User.IsSuperAdmin)
public static class TrackingRole
{
    public const string Admin = "Admin";
    public const string DivisionalManager = "Divisional Manager";
    public const string AreaManager = "Area Manager";
    public const string BranchManager = "Branch Manager";
    public const string AssistantBranchManager = "Assistant Branch Manager";
    public const string CreditOfficer = "Credit Officer";

    private static readonly string[] PostNames = [DivisionalManager, AreaManager, BranchManager, AssistantBranchManager, CreditOfficer];

    // Admin > Divisional Manager > Area Manager > Branch Manager = Assistant Branch Manager > Credit Officer
    public static int Rank(string role) => role switch
    {
        Admin => 5,
        DivisionalManager => 4,
        AreaManager => 3,
        BranchManager or AssistantBranchManager => 2,
        CreditOfficer => 1,
        _ => 0
    };

    public static bool IsTrackingPost(string postName) => PostNames.Contains(postName);

    public static IEnumerable<string> PostNamesBelow(string role) =>
        PostNames.Where(p => Rank(p) < Rank(role));
}

public record TrackingViewer(
    Guid UserId,
    Guid? EmployeeId,
    string Name,
    string Role,
    Guid? UnitId,
    string UnitName,
    IReadOnlyList<Guid> UnitIdsInScope,
    IReadOnlyList<Guid> BranchIds);
