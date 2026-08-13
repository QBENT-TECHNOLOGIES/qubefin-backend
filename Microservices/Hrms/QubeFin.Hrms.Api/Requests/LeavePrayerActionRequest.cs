namespace QubeFin.Hrms.Api.Requests
{
    public class LeavePrayerActionRequest
    {
        public Guid LeavePrayerId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
    }
}
