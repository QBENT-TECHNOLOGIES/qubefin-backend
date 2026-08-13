using Microsoft.AspNetCore.Http;

namespace QubeFin.Hrms.Application.LeavePrayers.Models
{
    public class LeavePrayerRequest
    {
        public Guid Id { get; set; }
        public Guid LeaveTypeId { get; set; }
        public int PrayerDays { get; set; }
        public IFormFile? Attachment { get; set; }
        public string? Remarks { get; set; }

    }
}
