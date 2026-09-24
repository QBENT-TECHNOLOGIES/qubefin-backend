namespace QubeFin.App.Api.Requests;

public record MenuRequest
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Target { get; set; }
    public Guid ParentId { get; set; }

    /// <summary>
    /// Only honoured on update: a newly created menu is always active. Defaults to true so a
    /// request that leaves it out can never deactivate a menu.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public List<PermissionRequest> Permissions { get; set; } = [];
}

public record PermissionRequest
{
    public Guid Id { get; set; }
    public int DisplayPosition { get; set; }
}