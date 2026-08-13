
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Salaries.Models
{
    public class SalaryComponentResponse : SalaryComponent
    {
       public SalaryComponentResponse(
            Guid id,
            string name,
            string code,
            Guid categoryId,
            string? categoryName,
            bool isTaxable,
            bool isPfapplicable,
            bool isEsiapplicable,
            bool isCtccomponent,
            bool isActive,
            int displayOrder,
            DateTime createdOn,
            Guid createdBy,
            DateTime? lastModifiedOn,
            Guid? lastModifiedBy) : base(
                id, name, code, categoryId, categoryName, isTaxable, isPfapplicable, isEsiapplicable, isCtccomponent, isActive, displayOrder, createdOn, createdBy, lastModifiedOn, lastModifiedBy)
        {
        }
        public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
    }
}
