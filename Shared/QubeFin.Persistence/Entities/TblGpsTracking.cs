using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGpsTracking
{
    public Guid Id { get; set; }

    public DateTime GpsTime { get; set; }

    public string LocationId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public bool IsMockLocation { get; set; }

    public int? BatteryPercentage { get; set; }

    public DateTime ReceivedAt { get; set; }

    public double? CourseDegree { get; set; }

    public double? Speed { get; set; }

    public double? Accuracy { get; set; }
}
