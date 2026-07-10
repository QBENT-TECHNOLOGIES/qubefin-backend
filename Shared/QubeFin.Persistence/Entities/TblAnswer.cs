using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAnswer
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string Descripton { get; set; } = null!;

    public int? Score { get; set; }

    public int Sequence { get; set; }

    public virtual TblQuestion Question { get; set; } = null!;
}
