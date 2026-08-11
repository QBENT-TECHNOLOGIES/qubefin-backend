using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblApprovalWorkflow
{
    public Guid Id { get; set; }

    public string Category { get; set; } = null!;

    public Guid? LeaveTypeId { get; set; }

    public Guid? OrganizationUnitTypeId { get; set; }

    public Guid? SalaryGradeId { get; set; }

    public Guid? PostId { get; set; }

    public int MinimumDays { get; set; }

    public int MaximumDays { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblUser? LastModifiedByNavigation { get; set; }

    public virtual TblLeaveType? LeaveType { get; set; }

    public virtual TblOrganizationUnitType? OrganizationUnitType { get; set; }

    public virtual TblPost? Post { get; set; }

    public virtual TblSalaryGrade? SalaryGrade { get; set; }

    public virtual ICollection<TblApprovalWorkflowStep> TblApprovalWorkflowSteps { get; set; } = new List<TblApprovalWorkflowStep>();
}
