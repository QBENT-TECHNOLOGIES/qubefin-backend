namespace QubeFin.App.Api.Requests;

public record MenuRequest
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Target { get; set; }
    public Guid ParentId { get; set; }
    public List<PermissionRequest> Permissions { get; set; } = [];
}

public record PermissionRequest
{
    public Guid Id { get; set; }
    public int DisplayPosition { get; set; }
}