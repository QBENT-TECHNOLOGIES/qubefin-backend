using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblImportType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string SpName { get; set; } = null!;

    public string? TemplateUrl { get; set; }

    public string Columns { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<TblImportJob> TblImportJobs { get; set; } = new List<TblImportJob>();
}
