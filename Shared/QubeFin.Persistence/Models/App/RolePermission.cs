namespace QubeFin.Persistence.Models.App;

public sealed class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }

    public RolePermission() { }

    public RolePermission(Guid roleId, Guid permissionId, Guid createdBy)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        CreatedBy = createdBy;
        CreatedOn = DateTime.UtcNow;
    }

    public static RolePermission Create(Guid roleId, Guid permissionId, Guid createdBy)
    {
        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };

        return rolePermission;
    }
}
