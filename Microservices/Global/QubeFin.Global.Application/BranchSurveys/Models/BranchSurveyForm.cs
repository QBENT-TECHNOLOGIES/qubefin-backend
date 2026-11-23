namespace QubeFin.Global.Application.BranchSurveys.Models
{
    // The Branch Location Survey form, grouped like the paper form (sections A to M).
    // It is used for both saving and loading.
    public class BranchSurveyForm
    {
        public DateOnly SurveyDate { get; set; }
        public GeographicSection Geographic { get; set; } = new();
        public AccessibilitySection Accessibility { get; set; } = new();
        public DemographicSection Demographic { get; set; } = new();
        public EconomicSection Economic { get; set; } = new();
        public MarketPotentialSection MarketPotential { get; set; } = new();
        public TransportationSection Transportation { get; set; } = new();
        public FinancialInclusionSection FinancialInclusion { get; set; } = new();
        public List<CompetitorRow> Competitors { get; set; } = new();
        public BusinessPotentialSection BusinessPotential { get; set; } = new();
        public RiskSection Risk { get; set; } = new();
        public ComplianceSection Compliance { get; set; } = new();
        public string? Recommendation { get; set; }
        public string? SurveyorComments { get; set; }
    }

    // A. Geographic Information
    public class GeographicSection
    {
        public Guid AdministrativeUnitId { get; set; }
        public string? ProposedOperationalArea { get; set; }
        public string? PoliceStation { get; set; }
        public string? PinCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? GeoTag { get; set; }
        public string? NearestLandmark { get; set; }
        public string? AdministrativeStatus { get; set; }
        public decimal? DistanceFromExistingWeGrowBranch { get; set; }     // km
        public decimal? DistanceFromDistrictHeadquarters { get; set; }     // km
    }

    // B. Accessibility Assessment
    public class AccessibilitySection
    {
        public string? RoadCondition { get; set; }
        public string? PublicTransportAvailability { get; set; }
        public string? RailwayConnectivity { get; set; }
        public string? BusConnectivity { get; set; }
        public string? MobileNetworkCoverage { get; set; }
        public string? InternetAvailability { get; set; }
        public string? ElectricitySupply { get; set; }
        public string? DrinkingWaterAvailability { get; set; }
        public string? SafetyOfArea { get; set; }
    }

    // C. Demographic Profile
    public class DemographicSection
    {
        public int? EstimatedPopulation { get; set; }
        public int? NumberOfHouseholds { get; set; }
        public decimal? AverageFamilySize { get; set; }
        public decimal? FemalePopulationPercent { get; set; }
        public decimal? LiteracyRate { get; set; }
        public int? WorkingPopulation { get; set; }                        // count
        public decimal? MinorityPopulationPercent { get; set; }
        public decimal? ScheduledCastePercent { get; set; }
        public decimal? ScheduledTribePercent { get; set; }
        public string? MigrationTrend { get; set; }
    }

    // D. Economic Profile (the activity fields are percentages)
    public class EconomicSection
    {
        public decimal? AgriculturePercent { get; set; }
        public decimal? AgriculturalLabour { get; set; }
        public decimal? DairyLivestock { get; set; }
        public decimal? SmallBusiness { get; set; }
        public decimal? PettyTrade { get; set; }
        public decimal? CottageSmallIndustries { get; set; }
        public decimal? TransportActivities { get; set; }
        public decimal? ServiceHolders { get; set; }
        public decimal? DailyWageEarners { get; set; }
        public string? OtherIncomeGeneratingActivities { get; set; }
        public string? MainCrop { get; set; }
        public string? PeakBusinessSeason { get; set; }
        public string? LeanSeason { get; set; }
        public string? OverallEconomicCondition { get; set; }
    }

    // E. Market Potential Assessment
    public class MarketPotentialSection
    {
        public int? EligibleHouseholds { get; set; }
        public int? PotentialWomenBorrowers { get; set; }
        public int? JlgPotential { get; set; }
        public int? IndividualBusinessLoansExpected { get; set; }
        public decimal? PortfolioYear1 { get; set; }
        public decimal? PortfolioYear2 { get; set; }
        public decimal? PortfolioYear3 { get; set; }
    }

    // F. Transportation Facilities (Yes / No)
    public class TransportationSection
    {
        public string? RailConnectivity { get; set; }
        public string? BusConnectivityAvailable { get; set; }
        public string? AutoTotoAvailability { get; set; }
        public string? RoadAccessibility { get; set; }
        public string? AccessibilityByMotorCycle { get; set; }
    }

    // G. Financial Inclusion Status
    public class FinancialInclusionSection
    {
        public int? NumberOfBanks { get; set; }
        public int? NumberOfRegionalRuralBanks { get; set; }
        public int? NumberOfCooperativeBanks { get; set; }
        public int? BankingCorrespondents { get; set; }
        public int? Atms { get; set; }
        public string? DigitalPaymentAcceptance { get; set; }
    }

    // H. Microfinance Competition Analysis (one row per institution)
    public class CompetitorRow
    {
        public string NameOfInstitution { get; set; } = string.Empty;
        public int? ApproxClients { get; set; }
        public decimal? ApproxPortfolio { get; set; }
        public decimal? ParPercent { get; set; }
    }

    // I. Business Potential Assessment
    public class BusinessPotentialSection
    {
        public int? EstimatedEligibleHouseholds { get; set; }
        public int? EstimatedWomenBorrowers { get; set; }
        public int? EstimatedNumberOfJlgCentres { get; set; }
        public decimal? EstimatedLoanPortfolioPotential { get; set; }
        public decimal? ExpectedMonthlyDisbursement { get; set; }
        public decimal? EstimatedCollectionEfficiency { get; set; }
    }

    // J. Risk Assessment
    public class RiskSection
    {
        // Natural risks
        public string? FloodRisk { get; set; }
        public string? CycloneRisk { get; set; }
        public string? LandslideRisk { get; set; }
        public string? DroughtRisk { get; set; }
        // Social risks
        public string? PoliticalDisturbanceRisk { get; set; }
        public string? CommunalIssuesRisk { get; set; }
        public string? MigrationRisk { get; set; }
        // Business risks
        public string? MultipleLendingRisk { get; set; }
        public string? CollectionRisk { get; set; }
        public string? FraudRisk { get; set; }
        public string? CompetitionRisk { get; set; }
    }

    // K. Compliance Verification (Yes / No)
    public class ComplianceSection
    {
        public string? AreaVisitedPhysically { get; set; }
        public string? GpsVerified { get; set; }
        public string? LocalReferencesVerified { get; set; }
        public string? ExistingCustomersContacted { get; set; }
        public string? CompetitorVerificationCompleted { get; set; }
        public string? PhotographsAttached { get; set; }
    }
}
