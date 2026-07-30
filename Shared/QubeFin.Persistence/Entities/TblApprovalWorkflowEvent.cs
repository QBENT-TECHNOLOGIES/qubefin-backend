using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblApprovalWorkflowEvent
{
    public Guid Id { get; set; }

    public string Category { get; set; } = null!;

    public Guid? LeaveTypeId { get; set; }

    public Guid? OrganizationUnitTypeId { get; set; }

    public Guid? SalaryGradeId { get; set; }

    public Guid? PostId { get; set; }

    public int MinimumDays { get; set; }

    public int? MaximumDays { get; set; }

    public int SequenceNo { get; set; }

    public Guid ReceiverPostId { get; set; }

    public bool IsRecommendEvent { get; set; }

    public bool IsApprovalEvent { get; set; }

    public string EventStatus { get; set; } = null!;

    public string EventButtonText { get; set; } = null!;

    public virtual TblLeaveType? LeaveType { get; set; }

    public virtual TblOrganizationUnitType? OrganizationUnitType { get; set; }

    public virtual TblPost? Post { get; set; }

    public virtual TblPost ReceiverPost { get; set; } = null!;

    public virtual TblSalaryGrade? SalaryGrade { get; set; }
}
