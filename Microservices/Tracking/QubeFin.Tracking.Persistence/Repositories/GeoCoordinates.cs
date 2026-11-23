using NetTopologySuite.Geometries;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

// geography points are stored as X = longitude, Y = latitude, SRID 4326; the API sends them as { lat, lng }, WGS-84, 6 decimals
public static class GeoCoordinates
{
    private const int Wgs84 = 4326;

    public static GeoPoint? ToGeoPoint(Geometry? location) =>
        location is Point point ? new GeoPoint(Math.Round((decimal)point.Y, 6), Math.Round((decimal)point.X, 6)) : null;

    public static GeoPoint? ToGeoPoint(decimal? latitude, decimal? longitude) =>
        latitude.HasValue && longitude.HasValue ? new GeoPoint(Math.Round(latitude.Value, 6), Math.Round(longitude.Value, 6)) : null;

    public static Point ToGeometry(GeoPoint point) =>
        new((double)Math.Round(point.Lng, 6), (double)Math.Round(point.Lat, 6)) { SRID = Wgs84 };
}
