using System;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Persistence.Mappers.Global;

public static class BranchSurveyMapper
{
    public static BranchSurvey ToDomain(TblBranchSurvey entity)
    {
        return BranchSurvey.Create(
            entity.Id,
            entity.SurveyId,
            entity.SurveyDate,
            MapGeographicInformation(entity),
            MapAccessibilityAssessment(entity),
            MapDemographicProfile(entity),
            MapEconomicProfile(entity),
            MapMarketPotential(entity),
            MapTransportationFacilities(entity),
            MapFinancialInclusionStatus(entity),
            MapMicrofinanceCompetition(entity),
            MapBusinessPotential(entity),
            MapRiskAssessment(entity),
            MapComplianceVerification(entity),
            MapRecommendation(entity),
            entity.CreatedBy
        );
    }

    private static BranchSurveyGeographicInformation MapGeographicInformation(TblBranchSurvey entity)
    {
        return new BranchSurveyGeographicInformation(
            entity.ProposedOperationalArea,
            entity.AdministrativeUnitId,
            entity.PinCode,
            entity.Latitude,
            entity.Longitude,
            entity.GeoTag,
            entity.NearestLandmark,
            entity.AdministrativeStatus,
            entity.DistanceFromExistingWeGrowBranch,
            entity.DistanceFromDistrictHeadquarters
        );
    }

    private static BranchSurveyAccessibilityAssessment MapAccessibilityAssessment(TblBranchSurvey entity)
    {
        return new BranchSurveyAccessibilityAssessment(
            entity.RoadCondition,
            entity.PublicTransportAvailability,
            entity.RailwayConnectivity,
            entity.BusConnectivity,
            entity.MobileNetworkCoverage,
            entity.InternetAvailability,
            entity.ElectricitySupply,
            entity.DrinkingWaterAvailability,
            entity.SafetyOfArea
        );
    }

    private static BranchSurveyDemographicProfile MapDemographicProfile(TblBranchSurvey entity)
    {
        return new BranchSurveyDemographicProfile(
            entity.EstimatedPopulation,
            entity.NumberOfHouseholds,
            entity.AverageFamilySize,
            entity.FemalePopulationPercent,
            entity.LiteracyRate,
            entity.WorkingPopulation,
            entity.MinorityPopulationPercent,
            entity.ScheduledCastePercent,
            entity.ScheduledTribePercent,
            entity.MigrationTrend
        );
    }

    private static BranchSurveyEconomicProfile MapEconomicProfile(TblBranchSurvey entity)
    {
        return new BranchSurveyEconomicProfile(
            entity.AgriculturePercent,
            entity.AgriculturalLabour,
            entity.DairyLivestock,
            entity.SmallBusiness,
            entity.PettyTrade,
            entity.CottageSmallIndustries,
            entity.TransportActivities,
            entity.ServiceHolders,
            entity.DailyWageEarners,
            entity.OtherIncomeGeneratingActivities,
            entity.MainCrop,
            entity.PeakBusinessSeason,
            entity.LeanSeason,
            entity.OverallEconomicCondition
        );
    }

    private static BranchSurveyMarketPotential MapMarketPotential(TblBranchSurvey entity)
    {
        return new BranchSurveyMarketPotential(
            entity.EligibleHouseholds,
            entity.PotentialWomenBorrowers,
            entity.Jlgpotential,
            entity.IndividualBusinessLoansExpected,
            entity.PortfolioYear1,
            entity.PortfolioYear2,
            entity.PortfolioYear3
        );
    }

    private static BranchSurveyTransportationFacilities MapTransportationFacilities(TblBranchSurvey entity)
    {
        return new BranchSurveyTransportationFacilities(
            entity.RailConnectivity,
            entity.BusConnectivityAvailable,
            entity.AutoTotoAvailability,
            entity.RoadAccessibility,
            entity.AccessibilityByMotorCycle
        );
    }

    private static BranchSurveyFinancialInclusionStatus MapFinancialInclusionStatus(TblBranchSurvey entity)
    {
        return new BranchSurveyFinancialInclusionStatus(
            entity.NumberOfBanks,
            entity.NumberOfRegionalRuralBanks,
            entity.NumberOfCooperativeBanks,
            entity.BankingCorrespondents,
            entity.Atms,
            entity.DigitalPaymentAcceptance
        );
    }

    private static BranchSurveyMicrofinanceCompetition MapMicrofinanceCompetition(TblBranchSurvey entity)
    {
        return new BranchSurveyMicrofinanceCompetition(
            entity.NameOfInstitution,
            entity.ApproxClients,
            entity.ApproxPortfolio,
            entity.Parpercent
        );
    }

    private static BranchSurveyBusinessPotential MapBusinessPotential(TblBranchSurvey entity)
    {
        return new BranchSurveyBusinessPotential(
            entity.EstimatedEligibleHouseholds,
            entity.EstimatedWomenBorrowers,
            entity.EstimatedNumberOfJlgsCentres,
            entity.EstimatedLoanPortfolioPotential,
            entity.ExpectedMonthlyDisbursement,
            entity.EstimatedCollectionEfficiency
        );
    }

    private static BranchSurveyRiskAssessment MapRiskAssessment(TblBranchSurvey entity)
    {
        return new BranchSurveyRiskAssessment(
            entity.FloodRisk,
            entity.CycloneRisk,
            entity.LandslideRisk,
            entity.DroughtRisk,
            entity.PoliticalDisturbanceRisk,
            entity.CommunalIssuesRisk,
            entity.MigrationRisk,
            entity.BusinessRisk,
            entity.MultipleLendingRisk,
            entity.CollectionRisk,
            entity.FraudRisk,
            entity.CompetitionRisk
        );
    }

    private static BranchSurveyComplianceVerification MapComplianceVerification(TblBranchSurvey entity)
    {
        return new BranchSurveyComplianceVerification(
            entity.AreaVisitedPhysically,
            entity.Gpsverified,
            entity.LocalReferencesVerified,
            entity.ExistingCustomersContacted,
            entity.CompetitorVerificationCompleted,
            entity.PhotographsAttached
        );
    }

    private static BranchSurveyRecommendation MapRecommendation(TblBranchSurvey entity)
    {
        return new BranchSurveyRecommendation(entity.Recommendation);
    }

    public static TblBranchSurvey ToEntity(this BranchSurvey branch)
    {
        return new TblBranchSurvey
        {
            Id = branch.Id,
            SurveyId = branch.SurveyId,
            SurveyDate = branch.SurveyDate,

            ProposedOperationalArea = branch.BranchSurveyGeographicInformation?.ProposedOperationalArea,
            AdministrativeUnitId = branch.BranchSurveyGeographicInformation?.AdministrativeUnitId ?? default,
            PinCode = branch.BranchSurveyGeographicInformation?.PinCode,
            Latitude = branch.BranchSurveyGeographicInformation?.Latitude,
            Longitude = branch.BranchSurveyGeographicInformation?.Longitude,
            GeoTag = branch.BranchSurveyGeographicInformation?.GeoTag,
            NearestLandmark = branch.BranchSurveyGeographicInformation?.NearestLandmark,
            AdministrativeStatus = branch.BranchSurveyGeographicInformation?.AdministrativeStatus,
            DistanceFromExistingWeGrowBranch = branch.BranchSurveyGeographicInformation?.DistanceFromExistingWeGrowBranch,
            DistanceFromDistrictHeadquarters = branch.BranchSurveyGeographicInformation?.DistanceFromDistrictHeadquarters,

            RoadCondition = branch.BranchSurveyAccessibilityAssessment?.RoadCondition,
            PublicTransportAvailability = branch.BranchSurveyAccessibilityAssessment?.PublicTransportAvailability,
            RailwayConnectivity = branch.BranchSurveyAccessibilityAssessment?.RailwayConnectivity,
            BusConnectivity = branch.BranchSurveyAccessibilityAssessment?.BusConnectivity,
            MobileNetworkCoverage = branch.BranchSurveyAccessibilityAssessment?.MobileNetworkCoverage,
            InternetAvailability = branch.BranchSurveyAccessibilityAssessment?.InternetAvailability,
            ElectricitySupply = branch.BranchSurveyAccessibilityAssessment?.ElectricitySupply,
            DrinkingWaterAvailability = branch.BranchSurveyAccessibilityAssessment?.DrinkingWaterAvailability,
            SafetyOfArea = branch.BranchSurveyAccessibilityAssessment?.SafetyOfArea,

            EstimatedPopulation = branch.BranchSurveyDemographicProfile?.EstimatedPopulation,
            NumberOfHouseholds = branch.BranchSurveyDemographicProfile?.NumberOfHouseholds,
            AverageFamilySize = branch.BranchSurveyDemographicProfile?.AverageFamilySize,
            FemalePopulationPercent = branch.BranchSurveyDemographicProfile?.FemalePopulationPercent,
            LiteracyRate = branch.BranchSurveyDemographicProfile?.LiteracyRate,
            WorkingPopulation = branch.BranchSurveyDemographicProfile?.WorkingPopulation,
            MinorityPopulationPercent = branch.BranchSurveyDemographicProfile?.MinorityPopulationPercent,
            ScheduledCastePercent = branch.BranchSurveyDemographicProfile?.ScheduledCastePercent,
            ScheduledTribePercent = branch.BranchSurveyDemographicProfile?.ScheduledTribePercent,
            MigrationTrend = branch.BranchSurveyDemographicProfile?.MigrationTrend,

            AgriculturePercent = branch.BranchSurveyEconomicProfile?.AgriculturePercent,
            AgriculturalLabour = branch.BranchSurveyEconomicProfile?.AgriculturalLabour,
            DairyLivestock = branch.BranchSurveyEconomicProfile?.DairyLivestock,
            SmallBusiness = branch.BranchSurveyEconomicProfile?.SmallBusiness,
            PettyTrade = branch.BranchSurveyEconomicProfile?.PettyTrade,
            CottageSmallIndustries = branch.BranchSurveyEconomicProfile?.CottageSmallIndustries,
            TransportActivities = branch.BranchSurveyEconomicProfile?.TransportActivities,
            ServiceHolders = branch.BranchSurveyEconomicProfile?.ServiceHolders,
            DailyWageEarners = branch.BranchSurveyEconomicProfile?.DailyWageEarners,
            OtherIncomeGeneratingActivities = branch.BranchSurveyEconomicProfile?.OtherIncomeGeneratingActivities,
            MainCrop = branch.BranchSurveyEconomicProfile?.MainCrop,
            PeakBusinessSeason = branch.BranchSurveyEconomicProfile?.PeakBusinessSeason,
            LeanSeason = branch.BranchSurveyEconomicProfile?.LeanSeason,
            OverallEconomicCondition = branch.BranchSurveyEconomicProfile?.OverallEconomicCondition,

            EligibleHouseholds = branch.BranchSurveyMarketPotential?.EligibleHouseholds,
            PotentialWomenBorrowers = branch.BranchSurveyMarketPotential?.PotentialWomenBorrowers,
            Jlgpotential = branch.BranchSurveyMarketPotential?.Jlgpotential,
            IndividualBusinessLoansExpected = branch.BranchSurveyMarketPotential?.IndividualBusinessLoansExpected,
            PortfolioYear1 = branch.BranchSurveyMarketPotential?.PortfolioYear1,
            PortfolioYear2 = branch.BranchSurveyMarketPotential?.PortfolioYear2,
            PortfolioYear3 = branch.BranchSurveyMarketPotential?.PortfolioYear3,

            RailConnectivity = branch.BranchSurveyTransportationFacilities?.RailConnectivity,
            BusConnectivityAvailable = branch.BranchSurveyTransportationFacilities?.BusConnectivityAvailable,
            AutoTotoAvailability = branch.BranchSurveyTransportationFacilities?.AutoTotoAvailability,
            RoadAccessibility = branch.BranchSurveyTransportationFacilities?.RoadAccessibility,
            AccessibilityByMotorCycle = branch.BranchSurveyTransportationFacilities?.AccessibilityByMotorCycle,

            NumberOfBanks = branch.BranchSurveyFinancialInclusionStatus?.NumberOfBanks,
            NumberOfRegionalRuralBanks = branch.BranchSurveyFinancialInclusionStatus?.NumberOfRegionalRuralBanks,
            NumberOfCooperativeBanks = branch.BranchSurveyFinancialInclusionStatus?.NumberOfCooperativeBanks,
            BankingCorrespondents = branch.BranchSurveyFinancialInclusionStatus?.BankingCorrespondents,
            Atms = branch.BranchSurveyFinancialInclusionStatus?.Atms,
            DigitalPaymentAcceptance = branch.BranchSurveyFinancialInclusionStatus?.DigitalPaymentAcceptance,

            NameOfInstitution = branch.BranchSurveyMicrofinanceCompetition?.NameOfInstitution,
            ApproxClients = branch.BranchSurveyMicrofinanceCompetition?.ApproxClients,
            ApproxPortfolio = branch.BranchSurveyMicrofinanceCompetition?.ApproxPortfolio,
            Parpercent = branch.BranchSurveyMicrofinanceCompetition?.Parpercent,

            EstimatedEligibleHouseholds = branch.BranchSurveyBusinessPotential?.EstimatedEligibleHouseholds,
            EstimatedWomenBorrowers = branch.BranchSurveyBusinessPotential?.EstimatedWomenBorrowers,
            EstimatedNumberOfJlgsCentres = branch.BranchSurveyBusinessPotential?.EstimatedNumberOfJlgsCentres,
            EstimatedLoanPortfolioPotential = branch.BranchSurveyBusinessPotential?.EstimatedLoanPortfolioPotential,
            ExpectedMonthlyDisbursement = branch.BranchSurveyBusinessPotential?.ExpectedMonthlyDisbursement,
            EstimatedCollectionEfficiency = branch.BranchSurveyBusinessPotential?.EstimatedCollectionEfficiency,

            FloodRisk = branch.BranchSurveyRiskAssessment?.FloodRisk,
            CycloneRisk = branch.BranchSurveyRiskAssessment?.CycloneRisk,
            LandslideRisk = branch.BranchSurveyRiskAssessment?.LandslideRisk,
            DroughtRisk = branch.BranchSurveyRiskAssessment?.DroughtRisk,
            PoliticalDisturbanceRisk = branch.BranchSurveyRiskAssessment?.PoliticalDisturbanceRisk,
            CommunalIssuesRisk = branch.BranchSurveyRiskAssessment?.CommunalIssuesRisk,
            MigrationRisk = branch.BranchSurveyRiskAssessment?.MigrationRisk,
            BusinessRisk = branch.BranchSurveyRiskAssessment?.BusinessRisk,
            MultipleLendingRisk = branch.BranchSurveyRiskAssessment?.MultipleLendingRisk,
            CollectionRisk = branch.BranchSurveyRiskAssessment?.CollectionRisk,
            FraudRisk = branch.BranchSurveyRiskAssessment?.FraudRisk,
            CompetitionRisk = branch.BranchSurveyRiskAssessment?.CompetitionRisk,

            AreaVisitedPhysically = branch.BranchSurveyComplianceVerification?.AreaVisitedPhysically,
            Gpsverified = branch.BranchSurveyComplianceVerification?.Gpsverified,
            LocalReferencesVerified = branch.BranchSurveyComplianceVerification?.LocalReferencesVerified,
            ExistingCustomersContacted = branch.BranchSurveyComplianceVerification?.ExistingCustomersContacted,
            CompetitorVerificationCompleted = branch.BranchSurveyComplianceVerification?.CompetitorVerificationCompleted,
            PhotographsAttached = branch.BranchSurveyComplianceVerification?.PhotographsAttached,

            Recommendation = branch.BranchSurveyRecommendation?.Recommendation,

            CreatedBy = branch.CreatedBy ?? default,
            CreatedOn = branch.CreatedDate ?? DateTime.UtcNow,
            LastModifiedBy = branch.LastModifiedBy,
            LastModifiedOn = branch.LastModifiedOn
        };
    }
}
