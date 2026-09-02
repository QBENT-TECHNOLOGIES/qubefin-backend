namespace QubeFin.App.Application.Menus.Models
{
    public class SaveRoleMenuRequest
    {
        public Guid MenuId { get; set; }
        public List<RoleMenuPermissionRequest> Roles { get; set; } = [];
        public List<UserMenuPermissionRequest> Users { get; set; } = [];
    }

    public class RoleMenuPermissionRequest
    {
        public Guid RoleId { get; set; }
        public List<Guid> MenuPermissionIds { get; set; } = [];
    }

    public class UserMenuPermissionRequest
    {
        public Guid UserId { get; set; }
        public List<Guid> MenuPermissionIds { get; set; } = [];
    }
}
