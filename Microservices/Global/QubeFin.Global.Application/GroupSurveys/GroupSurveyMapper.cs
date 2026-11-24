using NetTopologySuite.Geometries;
using QubeFin.Global.Application.GroupSurveys.Models;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.GroupSurveys
{
    // Moves data between the section-shaped form and the flat Tbl_GroupSurvey row.
    public static class GroupSurveyMapper
    {
        // SRID 4326 = normal GPS latitude / longitude.
        private const int GpsSrid = 4326;

        public static GroupSurveyForm ToForm(this TblGroupSurvey e, string? proposedArea)
        {
            return new GroupSurveyForm
            {
                SurveyDate = e.SurveyDate,
                Geographic = new GroupGeographicSection
                {
                    ProposedOperationalArea = proposedArea,
                    VillageLocality = e.VillageLocality,
                    GramPanchayatMunicipality = e.GramPanchayatMunicipality,
                    Block = e.Block,
                    // The location is stored as one point: X = longitude, Y = latitude.
                    Latitude = e.Location != null ? (decimal)e.Location.Coordinate.Y : null,
                    Longitude = e.Location != null ? (decimal)e.Location.Coordinate.X : null,
                    NearestLandmark = e.NearestLandmark
                },
                Profile = new GroupProfileSection
                {
                    ApproximateHouseholds = e.ApproximateHouseholds,
                    EstimatedPopulation = e.EstimatedPopulation,
                    MainOccupation = e.MainOccupation,
                    MainOccupationOther = e.MainOccupationOther
                },
                Environment = new GroupEnvironmentSection
                {
                    RoadAccessibility = e.RoadAccessibility,
                    AutoTotoAvailability = e.AutoTotoAvailability,
                    CommunityHarmony = e.CommunityHarmony,
                    WomenEconomicParticipation = e.WomenEconomicParticipation,
                    SocialAcceptanceOfMicrofinance = e.SocialAcceptanceOfMicrofinance,
                    PoliticalStability = e.PoliticalStability,
                    PoliticalDisturbance = e.PoliticalDisturbance,
                    LocalInfluenceOnLending = e.LocalInfluenceOnLending,
                    ElectionRelatedIssues = e.ElectionRelatedIssues
                },
                Risk = new GroupRiskSection
                {
                    FloodProne = e.FloodProne,
                    WaterLogging = e.WaterLogging,
                    DroughtProne = e.DroughtProne,
                    FlashFloodRisk = e.FlashFloodRisk,
                    CycloneRisk = e.CycloneRisk,
                    RiverErosion = e.RiverErosion,
                    OtherNaturalRisks = e.OtherNaturalRisks,
                    NumberOfMfis = e.NumberOfMfis,
                    SmallFinanceBanks = e.SmallFinanceBanks,
                    ShgFinancingInstitutions = e.ShgFinancingInstitutions
                },
                Market = new GroupMarketSection
                {
                    DistanceFromBranchKm = e.DistanceFromBranchKm,
                    DistanceFromNearestWeGrowCentreKm = e.DistanceFromNearestWeGrowCentreKm,
                    DistanceFromNearestCompetingCentreKm = e.DistanceFromNearestCompetingCentreKm,
                    AverageTravelTimeMinutes = e.AverageTravelTimeMinutes,
                    GroupsThatCanBeFormed = e.GroupsThatCanBeFormed,
                    EstimatedWomenBorrowers = e.EstimatedWomenBorrowers
                },
                Observation = new GroupObservationSection
                {
                    SuitableMeetingPlaces = e.SuitableMeetingPlaces,
                    CommunityResponseTowardsWeGrow = e.CommunityResponseTowardsWeGrow,
                    OtherObservations = e.OtherObservations,
                    SurveyorRecommendation = e.SurveyorRecommendation,
                    SurveyorComments = e.SurveyorComments
                }
            };
        }

        // The three place names are NOT taken from the form: the caller sets them from the survey area.
        public static void ApplyForm(this TblGroupSurvey e, GroupSurveyForm form)
        {
            e.SurveyDate = form.SurveyDate;

            var geo = form.Geographic;
            e.Location = geo.Latitude.HasValue && geo.Longitude.HasValue
                ? new Point((double)geo.Longitude.Value, (double)geo.Latitude.Value) { SRID = GpsSrid }
                : null;
            e.Latitude = geo.Latitude;
            e.Longitude = geo.Longitude;
            e.NearestLandmark = geo.NearestLandmark;

            var profile = form.Profile;
            e.ApproximateHouseholds = profile.ApproximateHouseholds;
            e.EstimatedPopulation = profile.EstimatedPopulation;
            e.MainOccupation = profile.MainOccupation;
            e.MainOccupationOther = profile.MainOccupation == "Other" ? profile.MainOccupationOther : null;

            var env = form.Environment;
            e.RoadAccessibility = env.RoadAccessibility;
            e.AutoTotoAvailability = env.AutoTotoAvailability;
            e.CommunityHarmony = env.CommunityHarmony;
            e.WomenEconomicParticipation = env.WomenEconomicParticipation;
            e.SocialAcceptanceOfMicrofinance = env.SocialAcceptanceOfMicrofinance;
            e.PoliticalStability = env.PoliticalStability;
            e.PoliticalDisturbance = env.PoliticalDisturbance;
            e.LocalInfluenceOnLending = env.LocalInfluenceOnLending;
            e.ElectionRelatedIssues = env.ElectionRelatedIssues;

            var risk = form.Risk;
            e.FloodProne = risk.FloodProne;
            e.WaterLogging = risk.WaterLogging;
            e.DroughtProne = risk.DroughtProne;
            e.FlashFloodRisk = risk.FlashFloodRisk;
            e.CycloneRisk = risk.CycloneRisk;
            e.RiverErosion = risk.RiverErosion;
            e.OtherNaturalRisks = risk.OtherNaturalRisks;
            e.NumberOfMfis = risk.NumberOfMfis;
            e.SmallFinanceBanks = risk.SmallFinanceBanks;
            e.ShgFinancingInstitutions = risk.ShgFinancingInstitutions;

            var market = form.Market;
            e.DistanceFromBranchKm = market.DistanceFromBranchKm;
            e.DistanceFromNearestWeGrowCentreKm = market.DistanceFromNearestWeGrowCentreKm;
            e.DistanceFromNearestCompetingCentreKm = market.DistanceFromNearestCompetingCentreKm;
            e.AverageTravelTimeMinutes = market.AverageTravelTimeMinutes;
            e.GroupsThatCanBeFormed = market.GroupsThatCanBeFormed;
            e.EstimatedWomenBorrowers = market.EstimatedWomenBorrowers;

            var obs = form.Observation;
            e.SuitableMeetingPlaces = obs.SuitableMeetingPlaces;
            e.CommunityResponseTowardsWeGrow = obs.CommunityResponseTowardsWeGrow;
            e.OtherObservations = obs.OtherObservations;
            e.SurveyorRecommendation = obs.SurveyorRecommendation;
            e.SurveyorComments = obs.SurveyorComments;
        }
    }
}
