using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.SurveyUnit.Models
{
    public class SurveyResponse
    {
        public Guid Id { get; set; }
        public string SurveyType { get; set; } = string.Empty;
        public string AssignmentNo { get; set; } = string.Empty;
        public DateOnly AssignmentDate { get; set; }
        public string ProposedArea { get; set; } = string.Empty;
        public Guid AdministrativeUnitId { get; set; }
        public DateOnly TentativeSubmissionDate { get; set; }
        public bool IsSurveyAccessed { get; set; }
        public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
        public List<SurveyAssignedResponse> SurveyAssigneds { get; set; } = new List<SurveyAssignedResponse>();
    }
    public class SurveyAssignedResponse
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public bool IsLead { get; set; }
        public DateTime AssignedDate { get; set; }
        public Guid AssignedBy { get; set; }
    }
    public class BranchSurveyResponse
    {

        public Guid Id { get; set; }

        public Guid SurveyId { get; set; }

        public DateOnly SurveyDate { get; set; }

        public string? ProposedOperationalArea { get; set; }

        public Guid AdministrativeUnitId { get; set; }

        public string? PinCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? GeoTag { get; set; }

        public string? NearestLandmark { get; set; }

        public string? AdministrativeStatus { get; set; }

        public decimal? DistanceFromExistingWeGrowBranch { get; set; }

        public decimal? DistanceFromDistrictHeadquarters { get; set; }

        public string? RoadCondition { get; set; }

        public string? PublicTransportAvailability { get; set; }

        public string? RailwayConnectivity { get; set; }

        public string? BusConnectivity { get; set; }

        public string? MobileNetworkCoverage { get; set; }

        public string? InternetAvailability { get; set; }

        public string? ElectricitySupply { get; set; }

        public string? DrinkingWaterAvailability { get; set; }

        public string? SafetyOfArea { get; set; }

        public int? EstimatedPopulation { get; set; }

        public int? NumberOfHouseholds { get; set; }

        public decimal? AverageFamilySize { get; set; }

        public decimal? FemalePopulationPercent { get; set; }

        public decimal? LiteracyRate { get; set; }

        public int? WorkingPopulation { get; set; }

        public decimal? MinorityPopulationPercent { get; set; }

        public decimal? ScheduledCastePercent { get; set; }

        public decimal? ScheduledTribePercent { get; set; }

        public string? MigrationTrend { get; set; }

        public decimal? AgriculturePercent { get; set; }

        public int? AgriculturalLabour { get; set; }

        public int? DairyLivestock { get; set; }

        public int? SmallBusiness { get; set; }

        public int? PettyTrade { get; set; }

        public int? CottageSmallIndustries { get; set; }

        public int? TransportActivities { get; set; }

        public int? ServiceHolders { get; set; }

        public int? DailyWageEarners { get; set; }

        public string? OtherIncomeGeneratingActivities { get; set; }

        public string? MainCrop { get; set; }

        public string? PeakBusinessSeason { get; set; }

        public string? LeanSeason { get; set; }

        public string? OverallEconomicCondition { get; set; }

        public int? EligibleHouseholds { get; set; }

        public int? PotentialWomenBorrowers { get; set; }

        public int? Jlgpotential { get; set; }

        public int? IndividualBusinessLoansExpected { get; set; }

        public decimal? PortfolioYear1 { get; set; }

        public decimal? PortfolioYear2 { get; set; }

        public decimal? PortfolioYear3 { get; set; }

        public string? RailConnectivity { get; set; }

        public string? BusConnectivityAvailable { get; set; }

        public string? AutoTotoAvailability { get; set; }

        public string? RoadAccessibility { get; set; }

        public string? AccessibilityByMotorCycle { get; set; }

        public int? NumberOfBanks { get; set; }

        public int? NumberOfRegionalRuralBanks { get; set; }

        public int? NumberOfCooperativeBanks { get; set; }

        public int? BankingCorrespondents { get; set; }

        public int? Atms { get; set; }

        public string? DigitalPaymentAcceptance { get; set; }

        public string? NameOfInstitution { get; set; }

        public int? ApproxClients { get; set; }

        public decimal? ApproxPortfolio { get; set; }

        public decimal? Parpercent { get; set; }

        public int? EstimatedEligibleHouseholds { get; set; }

        public int? EstimatedWomenBorrowers { get; set; }

        public int? EstimatedNumberOfJlgsCentres { get; set; }

        public decimal? EstimatedLoanPortfolioPotential { get; set; }

        public decimal? ExpectedMonthlyDisbursement { get; set; }

        public decimal? EstimatedCollectionEfficiency { get; set; }

        public string? FloodRisk { get; set; }

        public string? CycloneRisk { get; set; }

        public string? LandslideRisk { get; set; }

        public string? DroughtRisk { get; set; }

        public string? PoliticalDisturbanceRisk { get; set; }

        public string? CommunalIssuesRisk { get; set; }

        public string? MigrationRisk { get; set; }

        public string? BusinessRisk { get; set; }

        public string? MultipleLendingRisk { get; set; }

        public string? CollectionRisk { get; set; }

        public string? FraudRisk { get; set; }

        public string? CompetitionRisk { get; set; }

        public string? AreaVisitedPhysically { get; set; }

        public string? Gpsverified { get; set; }

        public string? LocalReferencesVerified { get; set; }

        public string? ExistingCustomersContacted { get; set; }

        public string? CompetitorVerificationCompleted { get; set; }

        public string? PhotographsAttached { get; set; }

        public string? Recommendation { get; set; }

        public bool IsSurveyorSubmit { get; set; }

        public DateTime? SurveyorSubmitOn { get; set; }

        public bool IsCommiteeSubmit { get; set; }

        public DateTime? CommiteeSubmitOn { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }
        public bool IsSubmitButtonVisible { get; set; }
        public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
    }
}
