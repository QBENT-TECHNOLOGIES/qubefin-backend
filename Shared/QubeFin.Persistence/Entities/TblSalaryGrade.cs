using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSalaryGrade
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual ICollection<TblSalaryStructure> TblSalaryStructures { get; set; } = new List<TblSalaryStructure>();
}
