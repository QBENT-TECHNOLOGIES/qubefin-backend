namespace QubeFin.Persistence.Models.Global;

public sealed class OrganizationUnitTree
{
    public Guid Id { get; set; }
    public Guid OrganizationUnitTypeId { get; set; }
    public string OrganizationUnitTypeIcon { get; set; } = string.Empty;
    public string OrganizationUnitTypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public List<OrganizationUnitTree> Children { get; set; } = [];
}

public static class OrganizationUnitTreeBuilder
{
    public static List<OrganizationUnitTree> Build(IEnumerable<OrganizationUnitTree> units)
    {
        var lookup = units.ToDictionary(x => x.Id);

        var roots = new List<OrganizationUnitTree>();

        foreach (var item in lookup.Values.OrderBy(m => m.Name))
        {
            if (item.ParentId == null)
            {
                roots.Add(item);
                continue;
            }

            if (lookup.TryGetValue(item.ParentId.Value, out var parent))
            {
                parent.Children.Add(item);
            }
        }

        return roots;
    }
}