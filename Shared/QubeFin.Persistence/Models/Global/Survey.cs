namespace QubeFin.Persistence.Models.Global
{
    public class Survey
    {
        private readonly List<SurveyAssigned> _surveyAssigneds = [];
        public Guid Id { get; private set; }
        public string SurveyType { get; private set; } = string.Empty;
        public string AssignmentNo { get; private set; } = string.Empty;
        public DateOnly AssignmentDate { get; private set; }
        public string ProposedArea { get; private set; } = string.Empty;
        public Guid AdministrativeUnitId { get; private set; }
        public DateOnly TentativeSubmissionDate { get; private set; }
        public string Status { get; private set; } = SurveyStatuses.Assigned;
        public bool SurveyFormAttached { get; private set; }
        // Only used by Group Survey: the branch the survey belongs to.
        public Guid? BranchOrganizationUnitId { get; private set; }
        // Set when a rejected survey is created again.
        public Guid? PreviousSurveyId { get; private set; }
        // The company of the survey: chosen for a Branch Survey, the branch's company for a Group Survey.
        public Guid? CompanyId { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public IReadOnlyCollection<SurveyAssigned> SurveyAssigneds => _surveyAssigneds;
        private Survey() { }

        // Used when a survey is loaded from the database.
        public Survey
            (
                Guid id,
                string surveyType,
                string assignmentNo,
                DateOnly assignmentDate,
                string proposedArea,
                Guid administrativeUnitId,
                DateOnly tentativeSubmissionDate,
                string status,
                bool surveyFormAttached,
                Guid? branchOrganizationUnitId,
                Guid? previousSurveyId,
                Guid? companyId,
                Guid createdBy,
                DateTime createdOn,
                Guid? lastModifiedBy,
                DateTime? lastModifiedOn,
                List<SurveyAssigned> surveyAssigneds
            )
        {
            Id = id;
            SurveyType = surveyType;
            AssignmentNo = assignmentNo;
            AssignmentDate = assignmentDate;
            ProposedArea = proposedArea;
            AdministrativeUnitId = administrativeUnitId;
            TentativeSubmissionDate = tentativeSubmissionDate;
            Status = status;
            SurveyFormAttached = surveyFormAttached;
            BranchOrganizationUnitId = branchOrganizationUnitId;
            PreviousSurveyId = previousSurveyId;
            CompanyId = companyId;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
            if (surveyAssigneds != null && surveyAssigneds.Any())
            {
                _surveyAssigneds.AddRange(surveyAssigneds);
            }
        }

        public static Survey Create(
            Guid id,
            string surveyType,
            string assignmentNo,
            DateOnly assignmentDate,
            string proposedArea,
            Guid administrativeUnitId,
            DateOnly tentativeSubmissionDate,
            bool surveyFormAttached,
            Guid? branchOrganizationUnitId,
            Guid? previousSurveyId,
            Guid? companyId,
            IEnumerable<SurveyAssigned> surveyAssigneds,
            Guid createdBy)
        {
            var survey = new Survey
            {
                Id = id,
                SurveyType = surveyType,
                AssignmentNo = assignmentNo,
                AssignmentDate = assignmentDate,
                ProposedArea = proposedArea,
                AdministrativeUnitId = administrativeUnitId,
                TentativeSubmissionDate = tentativeSubmissionDate,
                Status = SurveyStatuses.Assigned,
                SurveyFormAttached = surveyFormAttached,
                BranchOrganizationUnitId = branchOrganizationUnitId,
                PreviousSurveyId = previousSurveyId,
                CompanyId = companyId,
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now
            };
            survey._surveyAssigneds.AddRange(surveyAssigneds);
            return survey;
        }

        // Survey type, assignment number and status are not changed here.
        public void Update(
            DateOnly assignmentDate,
            string proposedArea,
            Guid administrativeUnitId,
            DateOnly tentativeSubmissionDate,
            bool surveyFormAttached,
            Guid? branchOrganizationUnitId,
            Guid? companyId,
            IEnumerable<SurveyAssigned> surveyAssigneds,
            Guid modifiedBy)
        {
            AssignmentDate = assignmentDate;
            ProposedArea = proposedArea;
            AdministrativeUnitId = administrativeUnitId;
            TentativeSubmissionDate = tentativeSubmissionDate;
            SurveyFormAttached = surveyFormAttached;
            BranchOrganizationUnitId = branchOrganizationUnitId;
            CompanyId = companyId;
            LastModifiedBy = modifiedBy;
            LastModifiedOn = DateTime.Now;
            _surveyAssigneds.Clear();
            _surveyAssigneds.AddRange(surveyAssigneds);
        }
    }
}
