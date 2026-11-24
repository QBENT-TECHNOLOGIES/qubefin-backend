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
        public string Status { get; set; } = string.Empty;
        public bool SurveyFormAttached { get; set; }
        public Guid? BranchOrganizationUnitId { get; set; }
        // Group Survey only.
        public string? BranchName { get; set; }
        public Guid? PreviousSurveyId { get; set; }
        public bool IsSurveyAccessed { get; set; }
        // True only for the Committee Lead who created the survey, until the Lead Surveyor submits it.
        public bool CanEditGeneral { get; set; }
        // True only for the Lead Surveyor of a Branch Survey, until the survey is submitted.
        public bool CanEditForm { get; set; }
        // Group Survey only: the group made from this survey (if any), and whether the user can create it now.
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? GroupCode { get; set; }
        public bool CanCreateGroup { get; set; }
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
