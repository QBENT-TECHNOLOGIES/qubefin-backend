using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveEvent
{
    public Guid Id { get; set; }

    public Guid LeaveTypeId { get; set; }

    public Guid OrganizationUnitTypeId { get; set; }

    public Guid SalaryGradeId { get; set; }

    public int MinimumDays { get; set; }

    public int? MaximumDays { get; set; }

    public int SequenceNo { get; set; }

    public Guid ReceiverPostId { get; set; }

    public bool IsRecommendEvent { get; set; }

    public bool IsApprovalEvent { get; set; }

    public string EventStatus { get; set; } = null!;

    public string EventButtonText { get; set; } = null!;

    public virtual TblLeaveType LeaveType { get; set; } = null!;

    public virtual TblOrganizationUnitType OrganizationUnitType { get; set; } = null!;

    public virtual TblPost ReceiverPost { get; set; } = null!;

    public virtual TblSalaryGrade SalaryGrade { get; set; } = null!;

    public virtual ICollection<TblLeaveRequestEvent> TblLeaveRequestEventLeaveEvents { get; set; } = new List<TblLeaveRequestEvent>();

    public virtual ICollection<TblLeaveRequestEvent> TblLeaveRequestEventNextLeaveEvents { get; set; } = new List<TblLeaveRequestEvent>();
}
