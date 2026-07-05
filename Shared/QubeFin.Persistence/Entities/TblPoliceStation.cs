using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPoliceStation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid DistrictId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblAdministrativeUnit District { get; set; } = null!;
}
