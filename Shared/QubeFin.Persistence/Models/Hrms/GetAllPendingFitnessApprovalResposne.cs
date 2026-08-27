namespace QubeFin.Persistence.Models.Hrms
{
    public class GetAllPendingFitnessApprovalResposne
    {
        public string EmployeeName { get; set;  } = string.Empty;
        public Guid LeaveRequestId { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateOnly FromDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set;}
    }
}
