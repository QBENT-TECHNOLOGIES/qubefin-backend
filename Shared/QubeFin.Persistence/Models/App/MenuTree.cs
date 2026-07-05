using QubeFin.Persistence.Models.Global;

namespace QubeFin.Persistence.Models.App;

public sealed class MenuTree
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Target { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayPosition { get; set; }
    public bool IsActive { get; set; }
    public List<MenuTree> Children { get; set; } = [];
}

public static class MenuTreeBuilder
{
    public static List<MenuTree> Build(IEnumerable<MenuTree> units)
    {
        var lookup = units.ToDictionary(x => x.Id);

        var roots = new List<MenuTree>();

        foreach (var item in lookup.Values.OrderBy(m => m.DisplayPosition))
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