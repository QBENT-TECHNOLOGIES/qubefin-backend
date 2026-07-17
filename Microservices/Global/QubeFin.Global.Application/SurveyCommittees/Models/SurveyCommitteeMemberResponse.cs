using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyCommittees.Models
{
    public class SurveyCommitteeMemberResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public bool IsLead { get; set; }
        public bool IsActive { get; set; }
        public DateOnly AssignedFrom { get; set; }
        public DateOnly? AssignedTo { get; set; }
        public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
    }
}
