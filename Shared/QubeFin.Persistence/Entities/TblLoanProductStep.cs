using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanProductStep
{
    public Guid Id { get; set; }

    public Guid LoanProductId { get; set; }

    public Guid ApplicationStepId { get; set; }

    public virtual TblApplicationStep ApplicationStep { get; set; } = null!;

    public virtual TblLoanProduct LoanProduct { get; set; } = null!;
}
