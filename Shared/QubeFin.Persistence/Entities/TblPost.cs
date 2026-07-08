using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPost
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<TblDesignation> TblDesignations { get; set; } = new List<TblDesignation>();

    public virtual ICollection<TblLoanProductQuestion> TblLoanProductQuestions { get; set; } = new List<TblLoanProductQuestion>();

    public virtual ICollection<TblLoanWorkflow> TblLoanWorkflows { get; set; } = new List<TblLoanWorkflow>();
}
