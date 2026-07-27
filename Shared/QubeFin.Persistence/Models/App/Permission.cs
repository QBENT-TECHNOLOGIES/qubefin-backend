namespace QubeFin.Persistence.Models.App;

public class Permission
{
    public Guid Id { get; private set; }

    public string PermissionToken { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Icon { get; private set; } = null!;

    public string BackgroundClass { get; private set; } = null!;

    public string IconClass { get; private set; } = null!;

    public int DisplayPosition { get; private set; }

    public Permission()
    {
    }

    public Permission(Guid id, string permissionToken, string description, string icon, string backgroundClass, string iconClass, int displayPosition)
    {
        Id = id;
        PermissionToken = permissionToken;
        Description = description;
        Icon = icon;
        BackgroundClass = backgroundClass;
        IconClass = iconClass;
        DisplayPosition = displayPosition;
    }
}
