using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSurveyCommittee
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public bool IsLead { get; set; }

    public bool IsActive { get; set; }

    public DateOnly AssignedFrom { get; set; }

    public DateOnly? AssignedTo { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;
}
