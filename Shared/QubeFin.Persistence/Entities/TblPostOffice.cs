using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPostOffice
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Pincode { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }
}
