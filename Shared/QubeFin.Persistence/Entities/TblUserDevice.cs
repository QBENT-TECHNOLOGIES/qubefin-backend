using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblUserDevice
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DeviceId { get; set; } = null!;

    public DateTime? AssignDate { get; set; }

    public bool IsReleased { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public Guid? ReleaseBy { get; set; }
}
