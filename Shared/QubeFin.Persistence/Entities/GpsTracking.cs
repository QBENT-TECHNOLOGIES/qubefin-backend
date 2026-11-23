using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Entities;

public partial class GpsTracking
{
    public Guid Id { get; set; }

    public DateTimeOffset GpsTime { get; set; }

    public string LocationId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public bool IsMockLocation { get; set; }

    public int? BatteryPercentage { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }

    public double? CourseDegree { get; set; }

    public Geometry? Location { get; set; }

    public double? Speed { get; set; }

    public double? Accuracy { get; set; }
}
