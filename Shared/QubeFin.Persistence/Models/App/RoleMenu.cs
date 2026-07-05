namespace QubeFin.Persistence.Models.App;

public class RoleMenu
{
    public Guid RoleId { get; private set; }
    public Guid MenuId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }

    public RoleMenu() { }

    public RoleMenu(Guid roleId, Guid menuId, Guid createdBy)
    {
        RoleId = roleId;
        MenuId = menuId;
        CreatedBy = createdBy;
        CreatedOn = DateTime.UtcNow;
    }

    public static RoleMenu Create(Guid roleId, Guid menuId, Guid createdBy)
    {
        var roleMenu = new RoleMenu
        {
            RoleId = roleId,
            MenuId = menuId,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };

        return roleMenu;
    }
}
