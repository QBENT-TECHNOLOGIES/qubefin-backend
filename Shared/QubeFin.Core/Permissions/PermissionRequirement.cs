using Microsoft.AspNetCore.Authorization;

namespace QubeFin.Core.Permissions;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
