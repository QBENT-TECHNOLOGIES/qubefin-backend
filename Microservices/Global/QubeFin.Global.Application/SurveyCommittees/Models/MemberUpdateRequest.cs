namespace QubeFin.Global.Application.SurveyCommittees.Models
{
    public class MemberUpdateRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsLead { get; set; }
        public DateOnly? AssignedTo { get; set; }
    }
}
