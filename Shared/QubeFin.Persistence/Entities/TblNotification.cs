using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblNotification
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

    public virtual TblDesignation Designation { get; set; } = null!;
}
