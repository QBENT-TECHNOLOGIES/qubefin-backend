using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblQuestion
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Category { get; set; } = null!;

    public string AnswerType { get; set; } = null!;

    public int Sequence { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblAnswer> TblAnswers { get; set; } = new List<TblAnswer>();

    public virtual ICollection<TblLoanProductQuestion> TblLoanProductQuestions { get; set; } = new List<TblLoanProductQuestion>();
}
