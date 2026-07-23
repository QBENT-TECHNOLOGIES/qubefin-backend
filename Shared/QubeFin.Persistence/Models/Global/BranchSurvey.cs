namespace QubeFin.Persistence.Models.Global
{
    public class BranchSurvey
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public DateOnly SurveyDate { get; set; }
        public BranchSurveyGeographicInformation BranchSurveyGeographicInformation { get; private set; } = default!;
        public BranchSurveyAccessibilityAssessment BranchSurveyAccessibilityAssessment { get; private set; } = default!;
        public BranchSurveyDemographicProfile BranchSurveyDemographicProfile { get; private set; } = default!;
        public BranchSurveyEconomicProfile BranchSurveyEconomicProfile { get; private set; } = default!;
        public BranchSurveyMarketPotential BranchSurveyMarketPotential { get; private set; } = default!;
        public BranchSurveyTransportationFacilities BranchSurveyTransportationFacilities { get; private set; } = default!;
        public BranchSurveyFinancialInclusionStatus BranchSurveyFinancialInclusionStatus { get; private set; } = default!;
        public BranchSurveyMicrofinanceCompetition BranchSurveyMicrofinanceCompetition { get; private set; } = default!;
        public BranchSurveyBusinessPotential BranchSurveyBusinessPotential { get; private set; } = default!;
        public BranchSurveyRiskAssessment BranchSurveyRiskAssessment { get; private set; } = default!;
        public BranchSurveyComplianceVerification BranchSurveyComplianceVerification { get; private set; } = default!;
        public BranchSurveyRecommendation BranchSurveyRecommendation { get; private set; } = default!;
        public Guid? CreatedBy { get; private set; }
        public DateTime? CreatedDate { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        private BranchSurvey() { }
        public BranchSurvey(Guid id, Guid surveyId, DateOnly surveyDate, BranchSurveyGeographicInformation geographicInformation,
            BranchSurveyAccessibilityAssessment accessibilityAssessment, BranchSurveyDemographicProfile demographicProfile,
            BranchSurveyEconomicProfile economicProfile, BranchSurveyMarketPotential marketPotential,
            BranchSurveyTransportationFacilities transportationFacilities, BranchSurveyFinancialInclusionStatus financialInclusionStatus,
            BranchSurveyMicrofinanceCompetition microfinanceCompetition, BranchSurveyBusinessPotential businessPotential,
            BranchSurveyRiskAssessment riskAssessment, BranchSurveyComplianceVerification complianceVerification,
            BranchSurveyRecommendation recommendation, Guid? createdBy)
        {
            Id = id;
            SurveyId = surveyId;
            SurveyDate = surveyDate;
            BranchSurveyGeographicInformation = geographicInformation;
            BranchSurveyAccessibilityAssessment = accessibilityAssessment;
            BranchSurveyDemographicProfile = demographicProfile;
            BranchSurveyEconomicProfile = economicProfile;
            BranchSurveyMarketPotential = marketPotential;
            BranchSurveyTransportationFacilities = transportationFacilities;
            BranchSurveyFinancialInclusionStatus = financialInclusionStatus;
            BranchSurveyMicrofinanceCompetition = microfinanceCompetition;
            BranchSurveyBusinessPotential = businessPotential;
            BranchSurveyRiskAssessment = riskAssessment;
            BranchSurveyComplianceVerification = complianceVerification;
            BranchSurveyRecommendation = recommendation;
            CreatedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
            LastModifiedBy = createdBy;
            LastModifiedOn = DateTime.UtcNow;
        }
        public static BranchSurvey Create(
                Guid id,
                Guid surveyId,
                DateOnly surveyDate,
                BranchSurveyGeographicInformation geographicInformation,
                BranchSurveyAccessibilityAssessment accessibilityAssessment,
                BranchSurveyDemographicProfile demographicProfile,
                BranchSurveyEconomicProfile economicProfile,
                BranchSurveyMarketPotential marketPotential,
                BranchSurveyTransportationFacilities transportationFacilities,
                BranchSurveyFinancialInclusionStatus financialInclusionStatus,
                BranchSurveyMicrofinanceCompetition microfinanceCompetition,
                BranchSurveyBusinessPotential businessPotential,
                BranchSurveyRiskAssessment routingAssessment,
                BranchSurveyComplianceVerification complianceVerification,
                BranchSurveyRecommendation recommendation,
                Guid? createdBy)
        {
            return new BranchSurvey
            {
                Id = id,
                SurveyId = surveyId,
                SurveyDate = surveyDate,
                BranchSurveyGeographicInformation = geographicInformation,
                BranchSurveyAccessibilityAssessment = accessibilityAssessment,
                BranchSurveyDemographicProfile = demographicProfile,
                BranchSurveyEconomicProfile = economicProfile,
                BranchSurveyMarketPotential = marketPotential,
                BranchSurveyTransportationFacilities = transportationFacilities,
                BranchSurveyFinancialInclusionStatus = financialInclusionStatus,
                BranchSurveyMicrofinanceCompetition = microfinanceCompetition,
                BranchSurveyBusinessPotential = businessPotential,
                BranchSurveyRiskAssessment = routingAssessment,
                BranchSurveyComplianceVerification = complianceVerification,
                BranchSurveyRecommendation = recommendation,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };
        }

        public void UpdateGeographicInformation(BranchSurveyGeographicInformation geographicInformation, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(geographicInformation);
            BranchSurveyGeographicInformation = geographicInformation;
            SetModified(modifiedBy);
        }

        public void UpdateAccessibilityAssessment(BranchSurveyAccessibilityAssessment accessibilityAssessment, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(accessibilityAssessment);
            BranchSurveyAccessibilityAssessment = accessibilityAssessment;
            SetModified(modifiedBy);
        }

        public void UpdateDemographicProfile(BranchSurveyDemographicProfile demographicProfile, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(demographicProfile);
            BranchSurveyDemographicProfile = demographicProfile;
            SetModified(modifiedBy);
        }

        public void UpdateEconomicProfile(BranchSurveyEconomicProfile economicProfile, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(economicProfile);
            BranchSurveyEconomicProfile = economicProfile;
            SetModified(modifiedBy);
        }

        public void UpdateMarketPotential(BranchSurveyMarketPotential marketPotential, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(marketPotential);
            BranchSurveyMarketPotential = marketPotential;
            SetModified(modifiedBy);
        }

        public void UpdateTransportationFacilities(BranchSurveyTransportationFacilities transportationFacilities, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(transportationFacilities);
            BranchSurveyTransportationFacilities = transportationFacilities;
            SetModified(modifiedBy);
        }

        public void UpdateFinancialInclusionStatus(BranchSurveyFinancialInclusionStatus financialInclusionStatus, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(financialInclusionStatus);
            BranchSurveyFinancialInclusionStatus = financialInclusionStatus;
            SetModified(modifiedBy);
        }

        public void UpdateMicrofinanceCompetition(BranchSurveyMicrofinanceCompetition microfinanceCompetition, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(microfinanceCompetition);
            BranchSurveyMicrofinanceCompetition = microfinanceCompetition;
            SetModified(modifiedBy);
        }

        public void UpdateBusinessPotential(BranchSurveyBusinessPotential businessPotential, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(businessPotential);
            BranchSurveyBusinessPotential = businessPotential;
            SetModified(modifiedBy);
        }

        public void UpdateRiskAssessment(BranchSurveyRiskAssessment riskAssessment, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(riskAssessment);
            BranchSurveyRiskAssessment = riskAssessment;
            SetModified(modifiedBy);
        }

        public void UpdateComplianceVerification(BranchSurveyComplianceVerification complianceVerification, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(complianceVerification);
            BranchSurveyComplianceVerification = complianceVerification;
            SetModified(modifiedBy);
        }

        public void UpdateRecommendation(BranchSurveyRecommendation recommendation, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(recommendation);
            BranchSurveyRecommendation = recommendation;
            SetModified(modifiedBy);
        }

        private void SetModified(Guid userId)
        {
            LastModifiedBy = userId;
            LastModifiedOn = DateTime.UtcNow;
        }
    }
    public class BranchSurveyGeographicInformation
    {
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
        public BranchSurveyGeographicInformation() { }
        public BranchSurveyGeographicInformation(string? proposedOperationalArea, Guid administrativeUnitId, string? pinCode, decimal? latitude, decimal? longitude, string? geoTag, string? nearestLandmark, string? administrativeStatus, decimal? distanceFromExistingWeGrowBranch, decimal? distanceFromDistrictHeadquarters)
        {
            ProposedOperationalArea = proposedOperationalArea;
            AdministrativeUnitId = administrativeUnitId;
            PinCode = pinCode;
            Latitude = latitude;
            Longitude = longitude;
            GeoTag = geoTag;
            NearestLandmark = nearestLandmark;
            AdministrativeStatus = administrativeStatus;
            DistanceFromExistingWeGrowBranch = distanceFromExistingWeGrowBranch;
            DistanceFromDistrictHeadquarters = distanceFromDistrictHeadquarters;
        }
    }
    public class BranchSurveyAccessibilityAssessment
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
        public BranchSurveyAccessibilityAssessment() { }
        public BranchSurveyAccessibilityAssessment(string? roadCondition, string? publicTransportAvailability, string? railwayConnectivity, string? busConnectivity, string? mobileNetworkCoverage, string? internetAvailability, string? electricitySupply, string? drinkingWaterAvailability, string? safetyOfArea)
        {
            RoadCondition = roadCondition;
            PublicTransportAvailability = publicTransportAvailability;
            RailwayConnectivity = railwayConnectivity;
            BusConnectivity = busConnectivity;
            MobileNetworkCoverage = mobileNetworkCoverage;
            InternetAvailability = internetAvailability;
            ElectricitySupply = electricitySupply;
            DrinkingWaterAvailability = drinkingWaterAvailability;
            SafetyOfArea = safetyOfArea;
        }
    }
    public class BranchSurveyDemographicProfile
    {
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
        public BranchSurveyDemographicProfile() { }
        public BranchSurveyDemographicProfile(int? estimatedPopulation, int? numberOfHouseholds, decimal? averageFamilySize, decimal? femalePopulationPercent, decimal? literacyRate, int? workingPopulation, decimal? minorityPopulationPercent, decimal? scheduledCastePercent, decimal? scheduledTribePercent, string? migrationTrend)
        {
            EstimatedPopulation = estimatedPopulation;
            NumberOfHouseholds = numberOfHouseholds;
            AverageFamilySize = averageFamilySize;
            FemalePopulationPercent = femalePopulationPercent;
            LiteracyRate = literacyRate;
            WorkingPopulation = workingPopulation;
            MinorityPopulationPercent = minorityPopulationPercent;
            ScheduledCastePercent = scheduledCastePercent;
            ScheduledTribePercent = scheduledTribePercent;
            MigrationTrend = migrationTrend;
        }
    }
    public class BranchSurveyEconomicProfile
    {
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
        public BranchSurveyEconomicProfile() { }
        public BranchSurveyEconomicProfile(decimal? agriculturePercent, int? agriculturalLabour, int? dairyLivestock, int? smallBusiness, int? pettyTrade, int? cottageSmallIndustries, int? transportActivities, int? serviceHolders, int? dailyWageEarners, string? otherIncomeGeneratingActivities, string? mainCrop, string? peakBusinessSeason, string? leanSeason, string? overallEconomicCondition)
        {
            AgriculturePercent = agriculturePercent;
            AgriculturalLabour = agriculturalLabour;
            DairyLivestock = dairyLivestock;
            SmallBusiness = smallBusiness;
            PettyTrade = pettyTrade;
            CottageSmallIndustries = cottageSmallIndustries;
            TransportActivities = transportActivities;
            ServiceHolders = serviceHolders;
            DailyWageEarners = dailyWageEarners;
            OtherIncomeGeneratingActivities = otherIncomeGeneratingActivities;
            MainCrop = mainCrop;
            PeakBusinessSeason = peakBusinessSeason;
            LeanSeason = leanSeason;
            OverallEconomicCondition = overallEconomicCondition;
        }
    }
    public class BranchSurveyMarketPotential
    {
        public int? EligibleHouseholds { get; set; }
        public int? PotentialWomenBorrowers { get; set; }
        public int? Jlgpotential { get; set; }
        public int? IndividualBusinessLoansExpected { get; set; }
        public decimal? PortfolioYear1 { get; set; }
        public decimal? PortfolioYear2 { get; set; }
        public decimal? PortfolioYear3 { get; set; }
        public BranchSurveyMarketPotential() { }
        public BranchSurveyMarketPotential(int? eligibleHouseholds, int? potentialWomenBorrowers, int? jlgpotential, int? individualBusinessLoansExpected, decimal? portfolioYear1, decimal? portfolioYear2, decimal? portfolioYear3)
        {
            EligibleHouseholds = eligibleHouseholds;
            PotentialWomenBorrowers = potentialWomenBorrowers;
            Jlgpotential = jlgpotential;
            IndividualBusinessLoansExpected = individualBusinessLoansExpected;
            PortfolioYear1 = portfolioYear1;
            PortfolioYear2 = portfolioYear2;
            PortfolioYear3 = portfolioYear3;
        }
    }
    public class BranchSurveyTransportationFacilities
    {
        public string? RailConnectivity { get; set; }
        public string? BusConnectivityAvailable { get; set; }
        public string? AutoTotoAvailability { get; set; }
        public string? RoadAccessibility { get; set; }
        public string? AccessibilityByMotorCycle { get; set; }
        public BranchSurveyTransportationFacilities() { }
        public BranchSurveyTransportationFacilities(string? railConnectivity, string? busConnectivityAvailable, string? autoTotoAvailability, string? roadAccessibility, string? accessibilityByMotorCycle)
        {
            RailConnectivity = railConnectivity;
            BusConnectivityAvailable = busConnectivityAvailable;
            AutoTotoAvailability = autoTotoAvailability;
            RoadAccessibility = roadAccessibility;
            AccessibilityByMotorCycle = accessibilityByMotorCycle;
        }
    }
    public class BranchSurveyFinancialInclusionStatus
    {
        public int? NumberOfBanks { get; set; }
        public int? NumberOfRegionalRuralBanks { get; set; }
        public int? NumberOfCooperativeBanks { get; set; }
        public int? BankingCorrespondents { get; set; }
        public int? Atms { get; set; }
        public string? DigitalPaymentAcceptance { get; set; }
        public BranchSurveyFinancialInclusionStatus() { }
        public BranchSurveyFinancialInclusionStatus(int? numberOfBanks, int? numberOfRegionalRuralBanks, int? numberOfCooperativeBanks, int? bankingCorrespondents, int? atms, string? digitalPaymentAcceptance)
        {
            NumberOfBanks = numberOfBanks;
            NumberOfRegionalRuralBanks = numberOfRegionalRuralBanks;
            NumberOfCooperativeBanks = numberOfCooperativeBanks;
            BankingCorrespondents = bankingCorrespondents;
            Atms = atms;
            DigitalPaymentAcceptance = digitalPaymentAcceptance;
        }
    }
    public class BranchSurveyMicrofinanceCompetition
    {
        public string? NameOfInstitution { get; set; }
        public int? ApproxClients { get; set; }
        public decimal? ApproxPortfolio { get; set; }
        public decimal? Parpercent { get; set; }
        public BranchSurveyMicrofinanceCompetition() { }
        public BranchSurveyMicrofinanceCompetition(string? nameOfInstitution, int? approxClients, decimal? approxPortfolio, decimal? parpercent)
        {
            NameOfInstitution = nameOfInstitution;
            ApproxClients = approxClients;
            ApproxPortfolio = approxPortfolio;
            Parpercent = parpercent;
        }
    }
    public class BranchSurveyBusinessPotential
    {
        public int? EstimatedEligibleHouseholds { get; set; }
        public int? EstimatedWomenBorrowers { get; set; }
        public int? EstimatedNumberOfJlgsCentres { get; set; }
        public decimal? EstimatedLoanPortfolioPotential { get; set; }
        public decimal? ExpectedMonthlyDisbursement { get; set; }
        public decimal? EstimatedCollectionEfficiency { get; set; }
        public BranchSurveyBusinessPotential() { }
        public BranchSurveyBusinessPotential(int? estimatedEligibleHouseholds, int? estimatedWomenBorrowers, int? estimatedNumberOfJlgsCentres, decimal? estimatedLoanPortfolioPotential, decimal? expectedMonthlyDisbursement, decimal? estimatedCollectionEfficiency)
        {
            EstimatedEligibleHouseholds = estimatedEligibleHouseholds;
            EstimatedWomenBorrowers = estimatedWomenBorrowers;
            EstimatedNumberOfJlgsCentres = estimatedNumberOfJlgsCentres;
            EstimatedLoanPortfolioPotential = estimatedLoanPortfolioPotential;
            ExpectedMonthlyDisbursement = expectedMonthlyDisbursement;
            EstimatedCollectionEfficiency = estimatedCollectionEfficiency;
        }
    }
    public class BranchSurveyRiskAssessment
    {
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
        public BranchSurveyRiskAssessment() { }
        public BranchSurveyRiskAssessment(string? floodRisk, string? cycloneRisk, string? landslideRisk, string? droughtRisk, string? politicalDisturbanceRisk, string? communalIssuesRisk, string? migrationRisk, string? businessRisk, string? multipleLendingRisk, string? collectionRisk, string? fraudRisk, string? competitionRisk)
        {
            FloodRisk = floodRisk;
            CycloneRisk = cycloneRisk;
            LandslideRisk = landslideRisk;
            DroughtRisk = droughtRisk;
            PoliticalDisturbanceRisk = politicalDisturbanceRisk;
            CommunalIssuesRisk = communalIssuesRisk;
            MigrationRisk = migrationRisk;
            BusinessRisk = businessRisk;
            MultipleLendingRisk = multipleLendingRisk;
            CollectionRisk = collectionRisk;
            FraudRisk = fraudRisk;
            CompetitionRisk = competitionRisk;
        }
    }
    public class BranchSurveyComplianceVerification
    {
        public string? AreaVisitedPhysically { get; set; }
        public string? Gpsverified { get; set; }
        public string? LocalReferencesVerified { get; set; }
        public string? ExistingCustomersContacted { get; set; }
        public string? CompetitorVerificationCompleted { get; set; }
        public string? PhotographsAttached { get; set; }
        public BranchSurveyComplianceVerification() { }
        public BranchSurveyComplianceVerification(string? areaVisitedPhysically, string? gpsverified, string? localReferencesVerified, string? existingCustomersContacted, string? competitorVerificationCompleted, string? photographsAttached)
        {
            AreaVisitedPhysically = areaVisitedPhysically;
            Gpsverified = gpsverified;
            LocalReferencesVerified = localReferencesVerified;
            ExistingCustomersContacted = existingCustomersContacted;
            CompetitorVerificationCompleted = competitorVerificationCompleted;
            PhotographsAttached = photographsAttached;
        }
    }
    public class BranchSurveyRecommendation
    {
        public string? Recommendation { get; set; }
        public BranchSurveyRecommendation() { }
        public BranchSurveyRecommendation(string? recommendation)
        {
            Recommendation = recommendation;
        }
    }
}
