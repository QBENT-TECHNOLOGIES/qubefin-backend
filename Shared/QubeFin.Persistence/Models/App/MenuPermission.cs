using QubeFin.Persistence.Models.Global;

namespace QubeFin.Persistence.Models.App;

public class MenuPermission
{
    public Guid Id { get; private set; }
    public Guid MenuId { get; private set; }
    public string PermissionToken { get; private set; } = null!;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }

    public MenuPermission()
    {
    }

    public MenuPermission(Guid id, Guid menuId, string permissionToken, Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
    {
        Id = id;
        MenuId = menuId;
        PermissionToken = permissionToken;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
    }

    public static MenuPermission Create(Guid menuId, string permissionToken, Guid userId)
    {
        return new MenuPermission
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            PermissionToken = permissionToken,
            CreatedBy = userId,
            CreatedOn = DateTime.Now
        };
    }
}
