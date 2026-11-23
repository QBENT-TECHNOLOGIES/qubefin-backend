using NetTopologySuite.Geometries;
using QubeFin.Global.Application.BranchSurveys.Models;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.BranchSurveys
{
    // Moves data between the section-shaped form and the flat Tbl_BranchSurvey row.
    public static class BranchSurveyMapper
    {
        // SRID 4326 = normal GPS latitude / longitude.
        private const int GpsSrid = 4326;

        public static BranchSurveyForm ToForm(this TblBranchSurvey entity)
        {
            return new BranchSurveyForm
            {
                SurveyDate = entity.SurveyDate,
                Geographic = new GeographicSection
                {
                    AdministrativeUnitId = entity.AdministrativeUnitId,
                    ProposedOperationalArea = entity.ProposedOperationalArea,
                    PoliceStation = entity.PoliceStation,
                    PinCode = entity.PinCode,
                    // The location is stored as one point: X = longitude, Y = latitude.
                    Latitude = entity.Location != null ? (decimal)entity.Location.Coordinate.Y : null,
                    Longitude = entity.Location != null ? (decimal)entity.Location.Coordinate.X : null,
                    GeoTag = entity.GeoTag,
                    NearestLandmark = entity.NearestLandmark,
                    AdministrativeStatus = entity.AdministrativeStatus,
                    DistanceFromExistingWeGrowBranch = entity.DistanceFromExistingWeGrowBranch,
                    DistanceFromDistrictHeadquarters = entity.DistanceFromDistrictHeadquarters
                },
                Accessibility = new AccessibilitySection
                {
                    RoadCondition = entity.RoadCondition,
                    PublicTransportAvailability = entity.PublicTransportAvailability,
                    RailwayConnectivity = entity.RailwayConnectivity,
                    BusConnectivity = entity.BusConnectivity,
                    MobileNetworkCoverage = entity.MobileNetworkCoverage,
                    InternetAvailability = entity.InternetAvailability,
                    ElectricitySupply = entity.ElectricitySupply,
                    DrinkingWaterAvailability = entity.DrinkingWaterAvailability,
                    SafetyOfArea = entity.SafetyOfArea
                },
                Demographic = new DemographicSection
                {
                    EstimatedPopulation = entity.EstimatedPopulation,
                    NumberOfHouseholds = entity.NumberOfHouseholds,
                    AverageFamilySize = entity.AverageFamilySize,
                    FemalePopulationPercent = entity.FemalePopulationPercent,
                    LiteracyRate = entity.LiteracyRate,
                    WorkingPopulation = entity.WorkingPopulation,
                    MinorityPopulationPercent = entity.MinorityPopulationPercent,
                    ScheduledCastePercent = entity.ScheduledCastePercent,
                    ScheduledTribePercent = entity.ScheduledTribePercent,
                    MigrationTrend = entity.MigrationTrend
                },
                Economic = new EconomicSection
                {
                    AgriculturePercent = entity.AgriculturePercent,
                    AgriculturalLabour = entity.AgriculturalLabour,
                    DairyLivestock = entity.DairyLivestock,
                    SmallBusiness = entity.SmallBusiness,
                    PettyTrade = entity.PettyTrade,
                    CottageSmallIndustries = entity.CottageSmallIndustries,
                    TransportActivities = entity.TransportActivities,
                    ServiceHolders = entity.ServiceHolders,
                    DailyWageEarners = entity.DailyWageEarners,
                    OtherIncomeGeneratingActivities = entity.OtherIncomeGeneratingActivities,
                    MainCrop = entity.MainCrop,
                    PeakBusinessSeason = entity.PeakBusinessSeason,
                    LeanSeason = entity.LeanSeason,
                    OverallEconomicCondition = entity.OverallEconomicCondition
                },
                MarketPotential = new MarketPotentialSection
                {
                    EligibleHouseholds = entity.EligibleHouseholds,
                    PotentialWomenBorrowers = entity.PotentialWomenBorrowers,
                    JlgPotential = entity.Jlgpotential,
                    IndividualBusinessLoansExpected = entity.IndividualBusinessLoansExpected,
                    PortfolioYear1 = entity.PortfolioYear1,
                    PortfolioYear2 = entity.PortfolioYear2,
                    PortfolioYear3 = entity.PortfolioYear3
                },
                Transportation = new TransportationSection
                {
                    RailConnectivity = entity.RailConnectivity,
                    BusConnectivityAvailable = entity.BusConnectivityAvailable,
                    AutoTotoAvailability = entity.AutoTotoAvailability,
                    RoadAccessibility = entity.RoadAccessibility,
                    AccessibilityByMotorCycle = entity.AccessibilityByMotorCycle
                },
                FinancialInclusion = new FinancialInclusionSection
                {
                    NumberOfBanks = entity.NumberOfBanks,
                    NumberOfRegionalRuralBanks = entity.NumberOfRegionalRuralBanks,
                    NumberOfCooperativeBanks = entity.NumberOfCooperativeBanks,
                    BankingCorrespondents = entity.BankingCorrespondents,
                    Atms = entity.Atms,
                    DigitalPaymentAcceptance = entity.DigitalPaymentAcceptance
                },
                Competitors = entity.TblBranchSurveyCompetitors
                    .OrderBy(c => c.CreatedOn)
                    .Select(c => new CompetitorRow
                    {
                        NameOfInstitution = c.NameOfInstitution,
                        ApproxClients = c.ApproxClients,
                        ApproxPortfolio = c.ApproxPortfolio,
                        ParPercent = c.Parpercent
                    }).ToList(),
                BusinessPotential = new BusinessPotentialSection
                {
                    EstimatedEligibleHouseholds = entity.EstimatedEligibleHouseholds,
                    EstimatedWomenBorrowers = entity.EstimatedWomenBorrowers,
                    EstimatedNumberOfJlgCentres = entity.EstimatedNumberOfJlgsCentres,
                    EstimatedLoanPortfolioPotential = entity.EstimatedLoanPortfolioPotential,
                    ExpectedMonthlyDisbursement = entity.ExpectedMonthlyDisbursement,
                    EstimatedCollectionEfficiency = entity.EstimatedCollectionEfficiency
                },
                Risk = new RiskSection
                {
                    FloodRisk = entity.FloodRisk,
                    CycloneRisk = entity.CycloneRisk,
                    LandslideRisk = entity.LandslideRisk,
                    DroughtRisk = entity.DroughtRisk,
                    PoliticalDisturbanceRisk = entity.PoliticalDisturbanceRisk,
                    CommunalIssuesRisk = entity.CommunalIssuesRisk,
                    MigrationRisk = entity.MigrationRisk,
                    MultipleLendingRisk = entity.MultipleLendingRisk,
                    CollectionRisk = entity.CollectionRisk,
                    FraudRisk = entity.FraudRisk,
                    CompetitionRisk = entity.CompetitionRisk
                },
                Compliance = new ComplianceSection
                {
                    AreaVisitedPhysically = entity.AreaVisitedPhysically,
                    GpsVerified = entity.Gpsverified,
                    LocalReferencesVerified = entity.LocalReferencesVerified,
                    ExistingCustomersContacted = entity.ExistingCustomersContacted,
                    CompetitorVerificationCompleted = entity.CompetitorVerificationCompleted,
                    PhotographsAttached = entity.PhotographsAttached
                },
                Recommendation = entity.Recommendation,
                SurveyorComments = entity.SurveyorComments
            };
        }

        // Copies the form into the row. Competitors are handled separately (see ToCompetitorEntities).
        public static void ApplyForm(this TblBranchSurvey entity, BranchSurveyForm form)
        {
            entity.SurveyDate = form.SurveyDate;

            var geo = form.Geographic;
            entity.AdministrativeUnitId = geo.AdministrativeUnitId;
            entity.ProposedOperationalArea = geo.ProposedOperationalArea;
            entity.PoliceStation = geo.PoliceStation;
            entity.PinCode = geo.PinCode;
            entity.Location = geo.Latitude.HasValue && geo.Longitude.HasValue
                ? new Point((double)geo.Longitude.Value, (double)geo.Latitude.Value) { SRID = GpsSrid }
                : null;
            entity.GeoTag = geo.GeoTag;
            entity.NearestLandmark = geo.NearestLandmark;
            entity.AdministrativeStatus = geo.AdministrativeStatus;
            entity.DistanceFromExistingWeGrowBranch = geo.DistanceFromExistingWeGrowBranch;
            entity.DistanceFromDistrictHeadquarters = geo.DistanceFromDistrictHeadquarters;

            var access = form.Accessibility;
            entity.RoadCondition = access.RoadCondition;
            entity.PublicTransportAvailability = access.PublicTransportAvailability;
            entity.RailwayConnectivity = access.RailwayConnectivity;
            entity.BusConnectivity = access.BusConnectivity;
            entity.MobileNetworkCoverage = access.MobileNetworkCoverage;
            entity.InternetAvailability = access.InternetAvailability;
            entity.ElectricitySupply = access.ElectricitySupply;
            entity.DrinkingWaterAvailability = access.DrinkingWaterAvailability;
            entity.SafetyOfArea = access.SafetyOfArea;

            var demo = form.Demographic;
            entity.EstimatedPopulation = demo.EstimatedPopulation;
            entity.NumberOfHouseholds = demo.NumberOfHouseholds;
            entity.AverageFamilySize = demo.AverageFamilySize;
            entity.FemalePopulationPercent = demo.FemalePopulationPercent;
            entity.LiteracyRate = demo.LiteracyRate;
            entity.WorkingPopulation = demo.WorkingPopulation;
            entity.MinorityPopulationPercent = demo.MinorityPopulationPercent;
            entity.ScheduledCastePercent = demo.ScheduledCastePercent;
            entity.ScheduledTribePercent = demo.ScheduledTribePercent;
            entity.MigrationTrend = demo.MigrationTrend;

            var eco = form.Economic;
            entity.AgriculturePercent = eco.AgriculturePercent;
            entity.AgriculturalLabour = eco.AgriculturalLabour;
            entity.DairyLivestock = eco.DairyLivestock;
            entity.SmallBusiness = eco.SmallBusiness;
            entity.PettyTrade = eco.PettyTrade;
            entity.CottageSmallIndustries = eco.CottageSmallIndustries;
            entity.TransportActivities = eco.TransportActivities;
            entity.ServiceHolders = eco.ServiceHolders;
            entity.DailyWageEarners = eco.DailyWageEarners;
            entity.OtherIncomeGeneratingActivities = eco.OtherIncomeGeneratingActivities;
            entity.MainCrop = eco.MainCrop;
            entity.PeakBusinessSeason = eco.PeakBusinessSeason;
            entity.LeanSeason = eco.LeanSeason;
            entity.OverallEconomicCondition = eco.OverallEconomicCondition;

            var market = form.MarketPotential;
            entity.EligibleHouseholds = market.EligibleHouseholds;
            entity.PotentialWomenBorrowers = market.PotentialWomenBorrowers;
            entity.Jlgpotential = market.JlgPotential;
            entity.IndividualBusinessLoansExpected = market.IndividualBusinessLoansExpected;
            entity.PortfolioYear1 = market.PortfolioYear1;
            entity.PortfolioYear2 = market.PortfolioYear2;
            entity.PortfolioYear3 = market.PortfolioYear3;

            var transport = form.Transportation;
            entity.RailConnectivity = transport.RailConnectivity;
            entity.BusConnectivityAvailable = transport.BusConnectivityAvailable;
            entity.AutoTotoAvailability = transport.AutoTotoAvailability;
            entity.RoadAccessibility = transport.RoadAccessibility;
            entity.AccessibilityByMotorCycle = transport.AccessibilityByMotorCycle;

            var finance = form.FinancialInclusion;
            entity.NumberOfBanks = finance.NumberOfBanks;
            entity.NumberOfRegionalRuralBanks = finance.NumberOfRegionalRuralBanks;
            entity.NumberOfCooperativeBanks = finance.NumberOfCooperativeBanks;
            entity.BankingCorrespondents = finance.BankingCorrespondents;
            entity.Atms = finance.Atms;
            entity.DigitalPaymentAcceptance = finance.DigitalPaymentAcceptance;

            var business = form.BusinessPotential;
            entity.EstimatedEligibleHouseholds = business.EstimatedEligibleHouseholds;
            entity.EstimatedWomenBorrowers = business.EstimatedWomenBorrowers;
            entity.EstimatedNumberOfJlgsCentres = business.EstimatedNumberOfJlgCentres;
            entity.EstimatedLoanPortfolioPotential = business.EstimatedLoanPortfolioPotential;
            entity.ExpectedMonthlyDisbursement = business.ExpectedMonthlyDisbursement;
            entity.EstimatedCollectionEfficiency = business.EstimatedCollectionEfficiency;

            var risk = form.Risk;
            entity.FloodRisk = risk.FloodRisk;
            entity.CycloneRisk = risk.CycloneRisk;
            entity.LandslideRisk = risk.LandslideRisk;
            entity.DroughtRisk = risk.DroughtRisk;
            entity.PoliticalDisturbanceRisk = risk.PoliticalDisturbanceRisk;
            entity.CommunalIssuesRisk = risk.CommunalIssuesRisk;
            entity.MigrationRisk = risk.MigrationRisk;
            entity.MultipleLendingRisk = risk.MultipleLendingRisk;
            entity.CollectionRisk = risk.CollectionRisk;
            entity.FraudRisk = risk.FraudRisk;
            entity.CompetitionRisk = risk.CompetitionRisk;

            var compliance = form.Compliance;
            entity.AreaVisitedPhysically = compliance.AreaVisitedPhysically;
            entity.Gpsverified = compliance.GpsVerified;
            entity.LocalReferencesVerified = compliance.LocalReferencesVerified;
            entity.ExistingCustomersContacted = compliance.ExistingCustomersContacted;
            entity.CompetitorVerificationCompleted = compliance.CompetitorVerificationCompleted;
            entity.PhotographsAttached = compliance.PhotographsAttached;

            entity.Recommendation = form.Recommendation;
            entity.SurveyorComments = form.SurveyorComments;
        }

        // Builds fresh competitor rows for a save. The old rows are deleted by the repository first.
        public static List<TblBranchSurveyCompetitor> ToCompetitorEntities(this BranchSurveyForm form, Guid branchSurveyId, Guid userId)
        {
            var now = DateTime.Now;

            // The table has no "order" column, so each row gets a slightly later CreatedOn to keep the order the user typed.
            return form.Competitors.Select((c, index) => new TblBranchSurveyCompetitor
            {
                Id = Guid.NewGuid(),
                BranchSurveyId = branchSurveyId,
                NameOfInstitution = c.NameOfInstitution,
                ApproxClients = c.ApproxClients,
                ApproxPortfolio = c.ApproxPortfolio,
                Parpercent = c.ParPercent,
                CreatedBy = userId,
                CreatedOn = now.AddMilliseconds(index * 10)
            }).ToList();
        }
    }
}
