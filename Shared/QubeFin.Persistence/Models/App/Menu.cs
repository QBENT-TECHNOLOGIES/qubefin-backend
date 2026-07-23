namespace QubeFin.Persistence.Models.App;

public class Menu
{
    private readonly List<Permission> _permissions = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Icon { get; private set; } = null!;
    public string? Target { get; private set; }
    public Guid? ParentId { get; private set; }
    public int DisplayPosition { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }

    public IReadOnlyCollection<Permission> Permissions =>   _permissions.AsReadOnly();

    public Menu() { }
    public Menu(Guid id, string name, string icon, string? target, Guid? parentId, int position, bool isActive, 
        Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
    {
        Id = id;
        Name = name;
        Icon = icon;
        Target = target;
        ParentId = parentId;
        DisplayPosition = position;
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
    }

    public static Menu Create(Guid id, string name, string icon, string? target, Guid? parentId, int position, Guid createdBy)
    {
        var menu = new Menu
        {
            Id = id,
            Name = name,
            Icon = icon,
            Target = target,
            ParentId = parentId,
            DisplayPosition = position,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };

        return menu;
    }

    public void Update(string name, string icon, string target, Guid? parentId, int position, Guid modifiedBy)
    {
        Name = name;
        Icon = icon;
        Target = target;
        ParentId = parentId;
        DisplayPosition = position;
        LastModifiedBy = modifiedBy;
        LastModifiedOn = DateTime.UtcNow;
    }
}
