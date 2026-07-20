using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.SurveyUnit.Models
{
    public class SurveyRequest
    {
        public Guid Id { get; set; }
        public string SurveyType { get; set; } = string.Empty;
        public DateOnly AssignmentDate { get; set; }
        public string ProposedArea { get; set; } = string.Empty;
        public Guid AdministrativeUnitId { get; set; }
        public DateOnly TentativeSubmissionDate { get; set; }
        public List<SurveyAssignedRequest> SurveyAssigneds { get; set; } = new List<SurveyAssignedRequest>();
    }
    public class SurveyAssignedRequest
    {
        public Guid EmployeeId { get; set; }
        public bool IsLead { get; set; }
    }
}
