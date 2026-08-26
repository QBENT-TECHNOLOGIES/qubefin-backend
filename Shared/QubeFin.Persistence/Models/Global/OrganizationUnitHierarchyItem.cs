namespace QubeFin.Persistence.Models.Global;

public class OrganizationUnitHierarchyItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
    public int Level { get; set; }
}
public class OragnizationDesignations
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public string PostName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public Guid? GradeId { get; set; }
    public string? GradeName { get; set; }
}
