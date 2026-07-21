namespace QubeFin.Global.Application.SurveyCommittees.Models
{
    public class MemberAddRequest
    {
        public Guid EmployeeId { get; set; }
        public bool IsLead { get; set; }
    }
}
