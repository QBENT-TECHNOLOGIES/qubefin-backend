using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.GroupSurveys
{
    // Checks that the Group Survey form is complete before the Lead Credit Officer submits it.
    // Every field is required EXCEPT: Other Observations and Comments (free text), and "Other occupation"
    // which is required only when the Main Occupation is "Other".
    public static class GroupSurveySubmitCheck
    {
        // Returns the names of the required fields that are still empty.
        public static List<string> GetMissingFields(TblGroupSurvey g)
        {
            var requiredFields = new (string Label, object? Value)[]
            {
                // Geographic Information
                ("Latitude and Longitude", g.Location),
                ("Nearest Landmark", g.NearestLandmark),
                // Demographic and Economic Profile
                ("Approximate Number of Households", g.ApproximateHouseholds),
                ("Estimated Population", g.EstimatedPopulation),
                ("Main Occupation", g.MainOccupation),
                // Transportation, Social and Political Environment
                ("Road Accessibility", g.RoadAccessibility),
                ("Auto / Toto Availability", g.AutoTotoAvailability),
                ("Community Harmony", g.CommunityHarmony),
                ("Participation of Women in Economic Activities", g.WomenEconomicParticipation),
                ("Social Acceptance of Microfinance", g.SocialAcceptanceOfMicrofinance),
                ("Political Stability", g.PoliticalStability),
                ("Any Political Disturbance", g.PoliticalDisturbance),
                ("Local Influence on Lending Activities", g.LocalInfluenceOnLending),
                ("Election-related Issues", g.ElectionRelatedIssues),
                // Climate and Disaster Risk
                ("Flood Prone Area", g.FloodProne),
                ("Water Logging", g.WaterLogging),
                ("Drought Prone", g.DroughtProne),
                ("Flash Flood Risk", g.FlashFloodRisk),
                ("Cyclone Risk", g.CycloneRisk),
                ("River Erosion", g.RiverErosion),
                ("Other Natural Risks", g.OtherNaturalRisks),
                // Competition Analysis
                ("Number of MFIs Operating", g.NumberOfMfis),
                ("Small Finance Banks", g.SmallFinanceBanks),
                ("SHG Financing Institutions", g.ShgFinancingInstitutions),
                // Distance Analysis
                ("Distance from the Branch", g.DistanceFromBranchKm),
                ("Distance from Nearest Existing WeGrow Centre", g.DistanceFromNearestWeGrowCentreKm),
                ("Distance from Nearest Competing Centre", g.DistanceFromNearestCompetingCentreKm),
                ("Average Travel Time", g.AverageTravelTimeMinutes),
                // Business Potential
                ("How many Groups can be formed", g.GroupsThatCanBeFormed),
                ("Estimated Women Borrowers", g.EstimatedWomenBorrowers),
                // Other Observations and Recommendation
                ("Availability of Suitable Centre Meeting Places", g.SuitableMeetingPlaces),
                ("Community Response towards WeGrow", g.CommunityResponseTowardsWeGrow),
                ("Recommendation", g.SurveyorRecommendation),
            };

            var missing = requiredFields
                .Where(f => f.Value is null || (f.Value is string text && string.IsNullOrWhiteSpace(text)))
                .Select(f => f.Label)
                .ToList();

            if (g.MainOccupation == "Other" && string.IsNullOrWhiteSpace(g.MainOccupationOther))
                missing.Add("Other Occupation");

            return missing;
        }
    }
}
