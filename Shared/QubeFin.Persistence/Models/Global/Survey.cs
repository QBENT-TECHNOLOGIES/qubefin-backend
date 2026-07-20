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
                string assignmentNo,
                DateOnly assignmentDate,
                string proposedArea,
                Guid administrativeUnitId,
                DateOnly tentativeSubmissionDate,
                Guid createdBy,
                DateTime createdOn,
                Guid? lastModifiedBy,
                DateTime? lastModifiedOn
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
        }
        public static Survey Create(Guid id, string surveyType, string assignmentNo, DateOnly assignmentDate, string proposedArea, Guid administrativeUnitId, DateOnly tentativeSubmissionDate, Guid createdBy)
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
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now,
            };
            return survey;
        }
        public void Update(string surveyType, DateOnly assignmentDate, string proposedArea, Guid administrativeUnitId, DateOnly tentativeSubmissionDate, Guid? lastModifiedBy)
        {
            SurveyType = surveyType;
            AssignmentDate = assignmentDate;
            ProposedArea = proposedArea;
            AdministrativeUnitId = administrativeUnitId;
            TentativeSubmissionDate = tentativeSubmissionDate;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = DateTime.Now;
        }

        public void AddSurveyAssigneds(SurveyAssigned surveyAssigned)
        {
            ArgumentNullException.ThrowIfNull(surveyAssigned);
            _surveyAssigneds.Add(surveyAssigned);
        }
        public void UpdateSurveyAssigneds(Guid surveyAssignedId, bool IsLead)
        {
            var surveyAssigned = _surveyAssigneds
                .FirstOrDefault(x => x.Id == surveyAssignedId);

            if (surveyAssigned is null)
            {
                throw new InvalidOperationException("Survey assigned not found.");
            }

            surveyAssigned.Update(IsLead);
        }
        public void RemoveSurveyAssigneds(Guid surveyAssignedId)
        {
            var surveyAssigned = _surveyAssigneds
                .FirstOrDefault(x => x.Id == surveyAssignedId);

            if (surveyAssigned != null)
            {
                _surveyAssigneds.Remove(surveyAssigned);
            }
        }
        public void ReplaceSurveyAssigneds(IEnumerable<SurveyAssigned> surveyAssigneds)
        {
            _surveyAssigneds.Clear();
            _surveyAssigneds.AddRange(surveyAssigneds);
        }
    }
}
