using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveRequestDocument
{
    public Guid Id { get; set; }

    public Guid LeaveRequestId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string DocumentFileNo { get; set; } = null!;

    public string DocumentFileSeq { get; set; } = null!;

    public DateTime? UploadedOn { get; set; }

    public Guid? UploadedBy { get; set; }

    public Guid LeaveRequestEventId { get; set; }
}
