namespace QubeFin.Global.Application.GroupSurveys.Models
{
    // The Group Formation Survey form ("Group Location Survey Report"), grouped like the paper form.
    // It is used for both saving and loading.
    public class GroupSurveyForm
    {
        public DateOnly SurveyDate { get; set; }
        public GroupGeographicSection Geographic { get; set; } = new();
        public GroupProfileSection Profile { get; set; } = new();
        public GroupEnvironmentSection Environment { get; set; } = new();
        public GroupRiskSection Risk { get; set; } = new();
        public GroupMarketSection Market { get; set; } = new();
        public GroupObservationSection Observation { get; set; } = new();
    }

    // Geographic Information. The proposed area and the three place names come from the survey assignment and are read-only.
    public class GroupGeographicSection
    {
        public string? ProposedOperationalArea { get; set; }
        public string? VillageLocality { get; set; }
        public string? GramPanchayatMunicipality { get; set; }
        public string? Block { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? NearestLandmark { get; set; }
    }

    // Demographic Profile + Economic Profile
    public class GroupProfileSection
    {
        public int? ApproximateHouseholds { get; set; }
        public int? EstimatedPopulation { get; set; }
        public string? MainOccupation { get; set; }
        public string? MainOccupationOther { get; set; }
    }

    // Transportation Facilities + Social Environment + Political Environment
    public class GroupEnvironmentSection
    {
        public string? RoadAccessibility { get; set; }
        public string? AutoTotoAvailability { get; set; }
        public string? CommunityHarmony { get; set; }
        public string? WomenEconomicParticipation { get; set; }
        public string? SocialAcceptanceOfMicrofinance { get; set; }
        public string? PoliticalStability { get; set; }
        public string? PoliticalDisturbance { get; set; }
        public string? LocalInfluenceOnLending { get; set; }
        public string? ElectionRelatedIssues { get; set; }
    }

    // Climate and Disaster Risk + Competition Analysis
    public class GroupRiskSection
    {
        public string? FloodProne { get; set; }
        public string? WaterLogging { get; set; }
        public string? DroughtProne { get; set; }
        public string? FlashFloodRisk { get; set; }
        public string? CycloneRisk { get; set; }
        public string? RiverErosion { get; set; }
        public string? OtherNaturalRisks { get; set; }
        public int? NumberOfMfis { get; set; }
        public int? SmallFinanceBanks { get; set; }
        public int? ShgFinancingInstitutions { get; set; }
    }

    // Distance Analysis + Business Potential Assessment
    public class GroupMarketSection
    {
        public decimal? DistanceFromBranchKm { get; set; }
        public decimal? DistanceFromNearestWeGrowCentreKm { get; set; }
        public decimal? DistanceFromNearestCompetingCentreKm { get; set; }
        public int? AverageTravelTimeMinutes { get; set; }
        public int? GroupsThatCanBeFormed { get; set; }
        public int? EstimatedWomenBorrowers { get; set; }
    }

    // Other Observations + Recommendation of the surveying Credit Officers
    public class GroupObservationSection
    {
        public string? SuitableMeetingPlaces { get; set; }
        public string? CommunityResponseTowardsWeGrow { get; set; }
        public string? OtherObservations { get; set; }
        public string? SurveyorRecommendation { get; set; }
        public string? SurveyorComments { get; set; }
    }
}
