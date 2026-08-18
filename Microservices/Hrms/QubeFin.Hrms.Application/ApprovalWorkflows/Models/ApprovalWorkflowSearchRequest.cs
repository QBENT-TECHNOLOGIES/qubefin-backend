using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models
{
    public class ApprovalWorkflowSearchRequest : SearchParam
    {
        public string? category { get; set; }
        public Guid? organizationUnitTypeId { get; set; }
    }
}
