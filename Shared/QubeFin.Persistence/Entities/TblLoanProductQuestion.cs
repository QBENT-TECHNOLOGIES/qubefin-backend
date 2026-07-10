using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanProductQuestion
{
    public Guid LoanProductId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid PostId { get; set; }

    public virtual TblLoanProduct LoanProduct { get; set; } = null!;

    public virtual TblPost Post { get; set; } = null!;

    public virtual TblQuestion Question { get; set; } = null!;
}
