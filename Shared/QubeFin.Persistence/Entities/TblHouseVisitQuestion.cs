using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblHouseVisitQuestion
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public int Sequence { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblHouseVisitAnswer> TblHouseVisitAnswers { get; set; } = new List<TblHouseVisitAnswer>();

    public virtual ICollection<TblLoanProductQuestion> TblLoanProductQuestions { get; set; } = new List<TblLoanProductQuestion>();
}
