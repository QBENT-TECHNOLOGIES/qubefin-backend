using NetTopologySuite.Geometries;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

// geography points are stored as X = longitude, Y = latitude; the API sends them as { lat, lng }, WGS-84, 6 decimals
internal static class GeoCoordinates
{
    public static GeoPoint? ToGeoPoint(Geometry? location) =>
        location is Point point ? new GeoPoint(Math.Round((decimal)point.Y, 6), Math.Round((decimal)point.X, 6)) : null;

    public static GeoPoint? ToGeoPoint(decimal? latitude, decimal? longitude) =>
        latitude.HasValue && longitude.HasValue ? new GeoPoint(Math.Round(latitude.Value, 6), Math.Round(longitude.Value, 6)) : null;
}
