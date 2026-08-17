using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Global
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid DesignationId { get; set; }
        public string Title { get; set; } = null!;
        public string? Icon { get; set; }
        public string Message { get; set; } = null!;
        public string? NotificationType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        protected Notification() { }
        public Notification(Guid id, Guid designationId, string title, string? icon, string message, string? notificationType, Guid? referenceId, string? referenceType, string? actionUrl, bool isRead, DateTime? readDate, Guid createdBy, DateTime createdOn)
        {
            Id = id;
            DesignationId = designationId;
            Title = title;
            Icon = icon;
            Message = message;
            NotificationType = notificationType;
            ReferenceId = referenceId;
            ReferenceType = referenceType;
            ActionUrl = actionUrl;
            IsRead = isRead;
            ReadDate = readDate;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
        }
    }
}
