using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.SurveyUnit.Models
{
    public class SurveyResponse
    {
        public Guid Id { get; set; }
        public string SurveyType { get; set; } = string.Empty;
        public string AssignmentNo { get; set; } = string.Empty;
        public DateOnly AssignmentDate { get; set; }
        public string ProposedArea { get; set; } = string.Empty;
        public Guid AdministrativeUnitId { get; set; }
        public DateOnly TentativeSubmissionDate { get; set; }
        public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
        public List<SurveyAssignedResponse> SurveyAssigneds { get; set; } = new List<SurveyAssignedResponse>();
    }
    public class SurveyAssignedResponse
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public bool IsLead { get; set; }
        public DateTime AssignedDate { get; set; }
        public Guid AssignedBy { get; set; }
    }
}
