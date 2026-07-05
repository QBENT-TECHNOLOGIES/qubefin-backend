using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSalaryStructureComponent
{
    public Guid Id { get; set; }

    public Guid SalaryStructureId { get; set; }

    public Guid SalaryComponentId { get; set; }

    public string CalculationMethod { get; set; } = null!;

    public decimal? Percentage { get; set; }

    public decimal? FixedAmount { get; set; }

    public string? Formula { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsEditable { get; set; }

    public bool IsMandatory { get; set; }

    public virtual TblSalaryComponent SalaryComponent { get; set; } = null!;

    public virtual TblSalaryStructure SalaryStructure { get; set; } = null!;
}
