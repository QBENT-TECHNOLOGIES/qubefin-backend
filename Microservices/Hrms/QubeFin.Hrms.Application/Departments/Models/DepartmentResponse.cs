using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Departments.Models
{
    public class DepartmentResponse: Department
    {
        public DepartmentResponse(Guid id, string name, bool isActive, DateTime? createdOn, Guid? createdBy, DateTime? lastModifiedOn, Guid? lastModifiedBy)
            : base(id, name, isActive, createdOn, createdBy, lastModifiedOn, lastModifiedBy)
        {
        }
        public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
    }
}
