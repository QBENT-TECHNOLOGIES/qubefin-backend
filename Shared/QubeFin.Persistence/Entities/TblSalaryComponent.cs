using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSalaryComponent
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public bool IsTaxable { get; set; }

    public bool IsPfapplicable { get; set; }

    public bool IsEsiapplicable { get; set; }

    public bool IsCtccomponent { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblSalaryComponentCategory Category { get; set; } = null!;

    public virtual ICollection<TblSalaryStructureComponent> TblSalaryStructureComponents { get; set; } = new List<TblSalaryStructureComponent>();
}
