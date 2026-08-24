namespace QubeFin.Persistence.Models.Hrms
{
    public class LeaveTypeWiseBalanceResponse
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public decimal? LeaveCredit { get; set; }
        public decimal? LeaveDebit { get; set; }
        public decimal? CurrentBalance { get; set; }
    }
}
