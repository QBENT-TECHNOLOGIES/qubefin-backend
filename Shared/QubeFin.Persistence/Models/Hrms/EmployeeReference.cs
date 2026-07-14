namespace QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Persistence.Models.Hrms
{
public class EmployeeReference
{
    public Guid Id { get; set; }
        public string DocumentCategory { get; set; } = null!;
        public string DocumentName { get; set; } = null!;
        public string? DocumentNo { get; set; }
        public DateOnly? ValidFrom { get; set; }
        public DateOnly? ValidTill { get; set; }
        public string? FileName { get; set; }
        public string? FileNo { get; set; }
    public Guid EmployeeId { get; set; }
    public string PersonName { get; set; } = null!;
    public string Mobile { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Occupation { get; set; }
    public string? HowDoYouKnow { get; set; }
}
