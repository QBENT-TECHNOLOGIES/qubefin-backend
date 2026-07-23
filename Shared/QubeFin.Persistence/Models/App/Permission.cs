namespace QubeFin.Persistence.Models.App;

public class Permission
{
    public string PermissionToken { get; private set; } = null!;
    public int DisplayPosition { get; private set; }

    public Permission()
    {
    }

    public Permission(string permissionToken, int displayPosition)
    {
        PermissionToken = permissionToken;
        DisplayPosition = displayPosition;
    }
}
