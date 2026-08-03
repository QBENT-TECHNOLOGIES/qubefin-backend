namespace QubeFin.Hrms.Api.Requests
{
    public class LeaveRequestActionRequest
    {
        public Guid LeaveRequestId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string? RejectedReason { get; set; }
    }
}
