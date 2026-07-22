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
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public IReadOnlyCollection<SurveyAssigned> SurveyAssigneds => _surveyAssigneds;
        private Survey() { }
        public Survey
            (
                Guid id,
                string surveyType,
                DateOnly assignmentDate,
                string proposedArea,
                Guid administrativeUnitId,
                DateOnly tentativeSubmissionDate,
                Guid createdBy,
                List<SurveyAssigned> surveyAssigneds
            )
        {
            Id = id;
            SurveyType = surveyType;
            AssignmentDate = assignmentDate;
            ProposedArea = proposedArea;
            AdministrativeUnitId = administrativeUnitId;
            TentativeSubmissionDate = tentativeSubmissionDate;
            CreatedBy = createdBy;
            LastModifiedBy = createdBy;
            if (surveyAssigneds != null && surveyAssigneds.Any())
            {
                _surveyAssigneds.AddRange(surveyAssigneds);
            }
        }
        public Survey
            (
                Guid id,
                string surveyType,
                string assignmentNo,
                DateOnly assignmentDate,
                string proposedArea,
                Guid administrativeUnitId,
                DateOnly tentativeSubmissionDate,
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
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
            if (surveyAssigneds != null && surveyAssigneds.Any())
            {
                _surveyAssigneds.AddRange(surveyAssigneds);
            }
        }
        public static Survey Create(Guid id, string surveyType, DateOnly assignmentDate, string proposedArea, Guid administrativeUnitId, DateOnly tentativeSubmissionDate, IEnumerable<SurveyAssigned> surveyAssigneds, Guid createdBy)
        {
            var survey = new Survey(id, surveyType, assignmentDate, proposedArea, administrativeUnitId, tentativeSubmissionDate, createdBy, surveyAssigneds.ToList());
            survey.AssignmentNo = new Random().Next(100000, 999999).ToString();
            return survey;
        }
    }
}
