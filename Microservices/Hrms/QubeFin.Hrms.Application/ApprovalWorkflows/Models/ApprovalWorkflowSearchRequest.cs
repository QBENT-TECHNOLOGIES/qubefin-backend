using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models
{
    public class ApprovalWorkflowSearchRequest : SearchParam
    {
        public string? Category { get; set; }
        public Guid? OrganizationUnitTypeId { get; set; }
        public Guid? SalaryGradeId { get; set; }
    }
}
