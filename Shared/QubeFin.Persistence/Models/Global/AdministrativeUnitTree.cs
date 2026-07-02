namespace QubeFin.Persistence.Models.Global;

public sealed class AdministrativeUnitTree
{
    public Guid Id { get; set; }

    public Guid AdministrativeUnitTypeId { get; set; }

    public string AdministrativeUnitTypeIcon { get; set; } = string.Empty;

    public string AdministrativeUnitTypeName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; }

    public List<AdministrativeUnitTree> Children { get; set; } = [];
}

public static class AdministrativeUnitTreeBuilder
{
    public static List<AdministrativeUnitTree> Build(IEnumerable<AdministrativeUnitTree> units)
    {
        var lookup = units.ToDictionary(x => x.Id);

        var roots = new List<AdministrativeUnitTree>();

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