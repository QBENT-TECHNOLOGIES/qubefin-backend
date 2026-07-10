using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanApplicationWorkflowDocument
{
    public Guid Id { get; set; }

    public Guid LoanApplicationWorkflowId { get; set; }

    public Guid DocumentId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileSequence { get; set; } = null!;

    public Guid UploadBy { get; set; }

    public DateTime UploadOn { get; set; }

    public virtual TblApplicatoinDocument Document { get; set; } = null!;

    public virtual TblLoanApplicationWorkflow LoanApplicationWorkflow { get; set; } = null!;
}
