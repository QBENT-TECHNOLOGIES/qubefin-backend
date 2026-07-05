using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSalaryStructure
{
    public Guid Id { get; set; }

    public Guid GradeId { get; set; }

    public decimal GrossAmount { get; set; }

    public DateOnly EffectiveFromDate { get; set; }

    public DateOnly? EffectiveToDate { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblSalaryGrade Grade { get; set; } = null!;

    public virtual ICollection<TblSalaryStructureComponent> TblSalaryStructureComponents { get; set; } = new List<TblSalaryStructureComponent>();
}
