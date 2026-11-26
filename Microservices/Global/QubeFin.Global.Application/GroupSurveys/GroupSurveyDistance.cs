using NetTopologySuite.Geometries;

namespace QubeFin.Global.Application.GroupSurveys
{
    // "Distance from the Branch" of the Group Survey form: the straight-line distance in km between the location
    // saved in step 1 of the form and the location of the branch. The server works it out; the screen only shows it.
    public static class GroupSurveyDistance
    {
        private const double EarthRadiusKm = 6371.0088;

        // Rounded to 2 decimals; null when either location is missing.
        public static decimal? Between(Geometry? surveyLocation, decimal? branchLatitude, decimal? branchLongitude)
        {
            if (surveyLocation is null || !branchLatitude.HasValue || !branchLongitude.HasValue)
                return null;

            double lat1 = ToRadians(surveyLocation.Coordinate.Y), lon1 = ToRadians(surveyLocation.Coordinate.X);
            double lat2 = ToRadians((double)branchLatitude.Value), lon2 = ToRadians((double)branchLongitude.Value);

            double dLat = lat2 - lat1, dLon = lon2 - lon1;
            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double km = 2 * EarthRadiusKm * Math.Asin(Math.Min(1, Math.Sqrt(h)));
            return Math.Round((decimal)km, 2);
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}