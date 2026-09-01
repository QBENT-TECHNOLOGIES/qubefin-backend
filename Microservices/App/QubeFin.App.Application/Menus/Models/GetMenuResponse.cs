using QubeFin.App.Application.Menus.Queries;
using QubeFin.Persistence.Models.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.Menus.Models
{
    public class GetMenuResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
        public string? Target { get; init; }
        public Guid? ParentId { get; init; }
        public int DisplayPosition { get; init; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; init; } = string.Empty;
        public DateTime CreatedOn { get; init; }
        public string? LastModifiedBy { get; init; }
        public DateTime? LastModifiedOn { get; init; }
        public IReadOnlyList<MenuHierarchyItem> Hierarchy { get; init; } = [];
        public IReadOnlyList<PermissionResponse> Permissions { get; init; } = [];
        public List<RoleMenuAssignmentResponse> Roles { get; init; } = [];
        public List<UserMenuAssignmentResponse> Users { get; init; } = [];
    }
    public sealed class UserMenuAssignmentResponse
    {
        public Guid UserId { get; init; }
        public Guid? EmployeeId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public List<Guid> MenuPermissionIds { get; init; } = [];
    }

    public sealed class RoleMenuAssignmentResponse
    {
        public Guid RoleId { get; init; }
        public string RoleName { get; init; } = string.Empty;
        public List<Guid> MenuPermissionIds { get; init; } = [];
        public bool IsSelected { get; init; }
    }

    public sealed record PermissionResponse
    {
        public Guid Id { get; init; }
        public string PermissionToken { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
        public string BackgroundClass { get; init; } = string.Empty;
        public string IconClass { get; init; } = string.Empty;
        public int DisplayPosition { get; init; }
        public bool Checked { get; init; }
    }
}
