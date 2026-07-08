using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanWorkflowDocument
{
    public Guid Id { get; set; }

    public Guid WorkflowId { get; set; }

    public Guid DocumentId { get; set; }

    public bool IsUploadPermission { get; set; }

    public bool IsMandatory { get; set; }

    public virtual TblApplicatoinDocument Document { get; set; } = null!;

    public virtual TblLoanWorkflow Workflow { get; set; } = null!;
}
