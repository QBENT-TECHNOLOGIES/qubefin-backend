using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblApplicatoinDocument
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Sequence { get; set; }

    public virtual ICollection<TblLoanWorkflowDocument> TblLoanWorkflowDocuments { get; set; } = new List<TblLoanWorkflowDocument>();
}
