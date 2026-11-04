namespace QubeFin.Persistence.Models.Tracking;

// Sent as { "lat": 26.528410, "lng": 89.210230 }, WGS-84, 6 decimals
public record GeoPoint(decimal Lat, decimal Lng);
