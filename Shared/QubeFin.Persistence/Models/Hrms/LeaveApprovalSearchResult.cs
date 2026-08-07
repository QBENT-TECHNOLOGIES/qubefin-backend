namespace QubeFin.Persistence.Models.Hrms
{
    public class LeaveApprovalSearchResult
    {
        public Guid? Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? LeaveType { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? Status { get; set;  }
        public int? TotalCount { get; set;  }
    }
}
