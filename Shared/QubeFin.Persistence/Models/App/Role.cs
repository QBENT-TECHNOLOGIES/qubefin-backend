namespace QubeFin.Persistence.Models.App;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }

    private readonly List<RoleMenu> _menus = [];
    public IReadOnlyCollection<RoleMenu> Menus =>
       _menus.AsReadOnly();

    private readonly List<RolePermission> _permissions = [];
    public IReadOnlyCollection<RolePermission> Permissions =>
       _permissions.AsReadOnly();

    private Role() { }

    public Role(Guid id, string name, bool isActive,
        Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
    }

    public static Role Create(Guid id, string name, Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));

        name = name.Trim();

        var role = new Role
        {
            Id = id,
            Name = name,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now
        };

        return role;
    }

    public void UpdateMenus(IEnumerable<Guid> menuIds, Guid modifiedBy)
    {
        ArgumentNullException.ThrowIfNull(menuIds);

        var menuSet = menuIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToHashSet();

        RemoveMissingMenus(menuSet);
        AddNewMenus(menuSet, modifiedBy);

        LastModifiedBy = modifiedBy;
        LastModifiedOn = DateTime.UtcNow;
    }

    public void ClearMenus(Guid modifiedBy)
    {
        _menus.Clear();

        LastModifiedBy = modifiedBy;
        LastModifiedOn = DateTime.UtcNow;
    }

    public bool HasMenu(Guid menuId)
    {
        return _menus.Any(x => x.MenuId == menuId);
    }

    public void UpdatePermissions(IEnumerable<Guid> permissionIds, Guid modifiedBy)
    {
        ArgumentNullException.ThrowIfNull(permissionIds);

        var permissionSet = permissionIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToHashSet();

        RemoveMissingPermissions(permissionSet);
        AddNewPermissions(permissionSet, modifiedBy);

        LastModifiedBy = modifiedBy;
        LastModifiedOn = DateTime.UtcNow;
    }

    public void ClearPermissions(Guid modifiedBy)
    {
        _permissions.Clear();

        LastModifiedBy = modifiedBy;
        LastModifiedOn = DateTime.UtcNow;
    }

    public bool HasPermission(Guid permissionId)
    {
        return _permissions.Any(x => x.PermissionId == permissionId);
    }

    private void RemoveMissingMenus(HashSet<Guid> menuIds)
    {
        _menus.RemoveAll(x => !menuIds.Contains(x.MenuId));
    }

    private void AddNewMenus(HashSet<Guid> menuIds, Guid createdBy)
    {
        foreach (var menuId in menuIds)
        {
            if (_menus.Any(x => x.MenuId == menuId))
                continue;

            _menus.Add(
                RoleMenu.Create(
                    Id,
                    menuId,
                    createdBy));
        }
    }

    private void RemoveMissingPermissions(HashSet<Guid> permissionIds)
    {
        _permissions.RemoveAll(x => !permissionIds.Contains(x.PermissionId));
    }

    private void AddNewPermissions(HashSet<Guid> permissionIds, Guid createdBy)
    {
        foreach (var permissionId in permissionIds)
        {
            if (_permissions.Any(x => x.PermissionId == permissionId))
                continue;

            _permissions.Add(
                RolePermission.Create(
                    Id,
                    permissionId,
                    createdBy));
        }
    }
}
