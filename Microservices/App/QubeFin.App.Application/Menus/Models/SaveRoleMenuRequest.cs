using QubeFin.App.Application.Menus.Commands;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Models
{
    public class SaveRoleMenuRequest
    {
        public Guid MenuId { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }
}
