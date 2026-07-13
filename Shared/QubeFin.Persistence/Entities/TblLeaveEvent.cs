using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveEvent
{
    public Guid Id { get; set; }

    public Guid ApproverRoleId { get; set; }

    public bool IsDecisionEvent { get; set; }

    public bool IsMemoEvent { get; set; }

    public bool IsForwardEvent { get; set; }

    public string EventStatusText { get; set; } = null!;

    public string ApplicantStatusText { get; set; } = null!;

    public string EventButtonText { get; set; } = null!;

    public string StatusColor { get; set; } = null!;
}
