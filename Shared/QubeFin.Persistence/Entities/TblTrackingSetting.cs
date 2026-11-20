using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblTrackingSetting
{
    public Guid Id { get; set; }

    public int NearHomeRadiusInMeters { get; set; }

    public int StationaryDurationInMinutes { get; set; }

    public int BranchOperatingRadiusInKm { get; set; }

    public int TaskLocationMatchInMeters { get; set; }

    public bool IsActive { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblUser? ModifiedByNavigation { get; set; }
}
