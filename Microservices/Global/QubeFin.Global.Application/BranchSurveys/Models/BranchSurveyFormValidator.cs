using FluentValidation;
using System.Linq.Expressions;

namespace QubeFin.Global.Application.BranchSurveys.Models
{
    // Checks that only need the form itself. A draft needs only the Survey Date; nothing else is mandatory.
    public class BranchSurveyFormValidator : AbstractValidator<BranchSurveyForm>
    {
        private const int MaxCompetitors = 10;

        public BranchSurveyFormValidator()
        {
            RuleFor(f => f.SurveyDate).NotEmpty().WithMessage("Date of survey is required.");
            RuleFor(f => f.Geographic.AdministrativeUnitId).NotEmpty().WithMessage("Area (administrative unit) is required.");

            ValidateGeographic();
            ValidateAccessibility();
            ValidateDemographic();
            ValidateEconomic();
            ValidateMarketPotential();
            ValidateTransportation();
            ValidateFinancialInclusion();
            ValidateCompetitors();
            ValidateBusinessPotential();
            ValidateRisk();
            ValidateCompliance();

            ValidateOption(f => f.Recommendation, BranchSurveyOptions.Recommendation, "Recommendation");
            RuleFor(f => f.SurveyorComments).MaximumLength(1000).WithMessage("Comments can have at most 1000 characters.");
        }

        private void ValidateGeographic()
        {
            RuleFor(f => f.Geographic.ProposedOperationalArea).MaximumLength(200).WithMessage("Proposed operational area can have at most 200 characters.");
            RuleFor(f => f.Geographic.PoliceStation).MaximumLength(100).WithMessage("Police station can have at most 100 characters.");
            RuleFor(f => f.Geographic.PinCode).MaximumLength(6).WithMessage("PIN code can have at most 6 characters.");
            RuleFor(f => f.Geographic.GeoTag).MaximumLength(200).WithMessage("Geo tag can have at most 200 characters.");
            RuleFor(f => f.Geographic.NearestLandmark).MaximumLength(200).WithMessage("Nearest landmark can have at most 200 characters.");
            ValidateOption(f => f.Geographic.AdministrativeStatus, BranchSurveyOptions.AdministrativeStatus, "Administrative status");
            ValidateNotNegative(f => f.Geographic.DistanceFromExistingWeGrowBranch, "Distance from existing WeGrow branch");
            ValidateNotNegative(f => f.Geographic.DistanceFromDistrictHeadquarters, "Distance from district headquarters");

            RuleFor(f => f.Geographic.Latitude).InclusiveBetween(-90m, 90m).When(f => f.Geographic.Latitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90.");
            RuleFor(f => f.Geographic.Longitude).InclusiveBetween(-180m, 180m).When(f => f.Geographic.Longitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180.");
            // The location is saved as one point, so latitude and longitude go together.
            RuleFor(f => f.Geographic)
                .Must(g => g.Latitude.HasValue == g.Longitude.HasValue)
                .WithMessage("Enter both latitude and longitude, or leave both empty.");
        }

        private void ValidateAccessibility()
        {
            ValidateOption(f => f.Accessibility.RoadCondition, BranchSurveyOptions.Rating, "Road condition");
            ValidateOption(f => f.Accessibility.PublicTransportAvailability, BranchSurveyOptions.Rating, "Public transport availability");
            ValidateOption(f => f.Accessibility.RailwayConnectivity, BranchSurveyOptions.Rating, "Railway connectivity");
            ValidateOption(f => f.Accessibility.BusConnectivity, BranchSurveyOptions.Rating, "Bus connectivity");
            ValidateOption(f => f.Accessibility.MobileNetworkCoverage, BranchSurveyOptions.Rating, "Mobile network coverage");
            ValidateOption(f => f.Accessibility.InternetAvailability, BranchSurveyOptions.Rating, "Internet availability");
            ValidateOption(f => f.Accessibility.ElectricitySupply, BranchSurveyOptions.Rating, "Electricity supply");
            ValidateOption(f => f.Accessibility.DrinkingWaterAvailability, BranchSurveyOptions.Rating, "Drinking water availability");
            ValidateOption(f => f.Accessibility.SafetyOfArea, BranchSurveyOptions.Rating, "Safety of area");
        }

        private void ValidateDemographic()
        {
            ValidateNotNegative(f => f.Demographic.EstimatedPopulation, "Estimated population");
            ValidateNotNegative(f => f.Demographic.NumberOfHouseholds, "Number of households");
            ValidateNotNegative(f => f.Demographic.AverageFamilySize, "Average family size");
            ValidatePercent(f => f.Demographic.FemalePopulationPercent, "Female population");
            ValidatePercent(f => f.Demographic.LiteracyRate, "Literacy rate");
            ValidateNotNegative(f => f.Demographic.WorkingPopulation, "Working population");
            ValidatePercent(f => f.Demographic.MinorityPopulationPercent, "Minority population");
            ValidatePercent(f => f.Demographic.ScheduledCastePercent, "Scheduled caste");
            ValidatePercent(f => f.Demographic.ScheduledTribePercent, "Scheduled tribe");
            ValidateOption(f => f.Demographic.MigrationTrend, BranchSurveyOptions.MigrationTrend, "Migration trend");
        }

        private void ValidateEconomic()
        {
            ValidatePercent(f => f.Economic.AgriculturePercent, "Agriculture");
            ValidatePercent(f => f.Economic.AgriculturalLabour, "Agricultural labour");
            ValidatePercent(f => f.Economic.DairyLivestock, "Dairy / livestock");
            ValidatePercent(f => f.Economic.SmallBusiness, "Small business");
            ValidatePercent(f => f.Economic.PettyTrade, "Petty trade");
            ValidatePercent(f => f.Economic.CottageSmallIndustries, "Cottage / small industries");
            ValidatePercent(f => f.Economic.TransportActivities, "Transport activities");
            ValidatePercent(f => f.Economic.ServiceHolders, "Service holders");
            ValidatePercent(f => f.Economic.DailyWageEarners, "Daily wage earners");
            // The nine activity shares are parts of 100 percent, so together they can be at most 100.
            RuleFor(f => f.Economic).Must(e =>
                (e.AgriculturePercent ?? 0) + (e.AgriculturalLabour ?? 0) + (e.DairyLivestock ?? 0) + (e.SmallBusiness ?? 0)
                + (e.PettyTrade ?? 0) + (e.CottageSmallIndustries ?? 0) + (e.TransportActivities ?? 0)
                + (e.ServiceHolders ?? 0) + (e.DailyWageEarners ?? 0) <= 100m)
                .WithMessage("The nine economic activity shares (Agriculture to Daily wage earners) can add up to at most 100.");
            RuleFor(f => f.Economic.OtherIncomeGeneratingActivities).MaximumLength(500).WithMessage("Other income generating activities can have at most 500 characters.");
            RuleFor(f => f.Economic.MainCrop).MaximumLength(200).WithMessage("Main crop can have at most 200 characters.");
            RuleFor(f => f.Economic.PeakBusinessSeason).MaximumLength(200).WithMessage("Peak business season can have at most 200 characters.");
            RuleFor(f => f.Economic.LeanSeason).MaximumLength(200).WithMessage("Lean season can have at most 200 characters.");
            ValidateOption(f => f.Economic.OverallEconomicCondition, BranchSurveyOptions.EconomicCondition, "Overall economic condition");
        }

        private void ValidateMarketPotential()
        {
            ValidateNotNegative(f => f.MarketPotential.EligibleHouseholds, "Eligible households");
            ValidateNotNegative(f => f.MarketPotential.PotentialWomenBorrowers, "Potential women borrowers");
            ValidateNotNegative(f => f.MarketPotential.JlgPotential, "JLG potential");
            ValidateNotNegative(f => f.MarketPotential.IndividualBusinessLoansExpected, "Individual business loans");
            ValidateNotNegative(f => f.MarketPotential.PortfolioYear1, "Expected portfolio year 1");
            ValidateNotNegative(f => f.MarketPotential.PortfolioYear2, "Expected portfolio year 2");
            ValidateNotNegative(f => f.MarketPotential.PortfolioYear3, "Expected portfolio year 3");
        }

        private void ValidateTransportation()
        {
            ValidateOption(f => f.Transportation.RailConnectivity, BranchSurveyOptions.YesNo, "Rail connectivity");
            ValidateOption(f => f.Transportation.BusConnectivityAvailable, BranchSurveyOptions.YesNo, "Bus connectivity");
            ValidateOption(f => f.Transportation.AutoTotoAvailability, BranchSurveyOptions.YesNo, "Auto / Toto availability");
            ValidateOption(f => f.Transportation.RoadAccessibility, BranchSurveyOptions.YesNo, "Road accessibility");
            ValidateOption(f => f.Transportation.AccessibilityByMotorCycle, BranchSurveyOptions.YesNo, "Motorcycle accessibility");
        }

        private void ValidateFinancialInclusion()
        {
            ValidateNotNegative(f => f.FinancialInclusion.NumberOfBanks, "Number of banks");
            ValidateNotNegative(f => f.FinancialInclusion.NumberOfRegionalRuralBanks, "Number of regional rural banks");
            ValidateNotNegative(f => f.FinancialInclusion.NumberOfCooperativeBanks, "Number of cooperative banks");
            ValidateNotNegative(f => f.FinancialInclusion.BankingCorrespondents, "Banking correspondents");
            ValidateNotNegative(f => f.FinancialInclusion.Atms, "ATMs");
            ValidateOption(f => f.FinancialInclusion.DigitalPaymentAcceptance, BranchSurveyOptions.YesNo, "Digital payment acceptance");
        }

        private void ValidateCompetitors()
        {
            RuleFor(f => f.Competitors.Count).LessThanOrEqualTo(MaxCompetitors)
                .WithMessage($"You can add at most {MaxCompetitors} competitors.");
            RuleForEach(f => f.Competitors).ChildRules(competitor =>
            {
                competitor.RuleFor(c => c.NameOfInstitution).NotEmpty().WithMessage("Competitor name is required.");
                competitor.RuleFor(c => c.NameOfInstitution).MaximumLength(200).WithMessage("Competitor name can have at most 200 characters.");
                competitor.RuleFor(c => c.ApproxClients).GreaterThanOrEqualTo(0).When(c => c.ApproxClients.HasValue)
                    .WithMessage("Competitor clients cannot be negative.");
                competitor.RuleFor(c => c.ApproxPortfolio).GreaterThanOrEqualTo(0).When(c => c.ApproxPortfolio.HasValue)
                    .WithMessage("Competitor portfolio cannot be negative.");
                competitor.RuleFor(c => c.ParPercent).InclusiveBetween(0m, 100m).When(c => c.ParPercent.HasValue)
                    .WithMessage("Competitor PAR % must be between 0 and 100.");
            });
        }

        private void ValidateBusinessPotential()
        {
            ValidateNotNegative(f => f.BusinessPotential.EstimatedEligibleHouseholds, "Estimated eligible households");
            ValidateNotNegative(f => f.BusinessPotential.EstimatedWomenBorrowers, "Estimated women borrowers");
            ValidateNotNegative(f => f.BusinessPotential.EstimatedNumberOfJlgCentres, "Estimated number of JLG centres");
            ValidateNotNegative(f => f.BusinessPotential.EstimatedLoanPortfolioPotential, "Estimated loan portfolio potential");
            ValidateNotNegative(f => f.BusinessPotential.ExpectedMonthlyDisbursement, "Expected monthly disbursement");
            ValidatePercent(f => f.BusinessPotential.EstimatedCollectionEfficiency, "Estimated collection efficiency");
        }

        private void ValidateRisk()
        {
            ValidateOption(f => f.Risk.FloodRisk, BranchSurveyOptions.NaturalRisk, "Flood risk");
            ValidateOption(f => f.Risk.CycloneRisk, BranchSurveyOptions.NaturalRisk, "Cyclone risk");
            ValidateOption(f => f.Risk.LandslideRisk, BranchSurveyOptions.NaturalRisk, "Landslide risk");
            ValidateOption(f => f.Risk.DroughtRisk, BranchSurveyOptions.NaturalRisk, "Drought risk");
            ValidateOption(f => f.Risk.PoliticalDisturbanceRisk, BranchSurveyOptions.RiskLevel, "Political disturbance risk");
            ValidateOption(f => f.Risk.CommunalIssuesRisk, BranchSurveyOptions.RiskLevel, "Communal issues risk");
            ValidateOption(f => f.Risk.MigrationRisk, BranchSurveyOptions.RiskLevel, "Migration risk");
            ValidateOption(f => f.Risk.MultipleLendingRisk, BranchSurveyOptions.RiskLevel, "Multiple lending risk");
            ValidateOption(f => f.Risk.CollectionRisk, BranchSurveyOptions.RiskLevel, "Collection risk");
            ValidateOption(f => f.Risk.FraudRisk, BranchSurveyOptions.RiskLevel, "Fraud risk");
            ValidateOption(f => f.Risk.CompetitionRisk, BranchSurveyOptions.RiskLevel, "Competition risk");
        }

        private void ValidateCompliance()
        {
            ValidateOption(f => f.Compliance.AreaVisitedPhysically, BranchSurveyOptions.YesNo, "Area visited physically");
            ValidateOption(f => f.Compliance.GpsVerified, BranchSurveyOptions.YesNo, "GPS verified");
            ValidateOption(f => f.Compliance.LocalReferencesVerified, BranchSurveyOptions.YesNo, "Local references verified");
            ValidateOption(f => f.Compliance.ExistingCustomersContacted, BranchSurveyOptions.YesNo, "Existing customers contacted");
            ValidateOption(f => f.Compliance.CompetitorVerificationCompleted, BranchSurveyOptions.YesNo, "Competitor verification completed");
            ValidateOption(f => f.Compliance.PhotographsAttached, BranchSurveyOptions.YesNo, "Photographs attached");
        }

        // ---- Small helpers so every field has the same kind of message ----

        private void ValidateOption(Expression<Func<BranchSurveyForm, string?>> field, string[] allowedValues, string fieldName)
        {
            RuleFor(field)
                .Must(value => string.IsNullOrEmpty(value) || allowedValues.Contains(value))
                .WithMessage($"{fieldName} must be one of: {string.Join(", ", allowedValues)}.");
        }

        // An empty (null) value is always accepted: a draft does not need every field.
        private void ValidatePercent(Expression<Func<BranchSurveyForm, decimal?>> field, string fieldName)
        {
            RuleFor(field).InclusiveBetween(0m, 100m).WithMessage($"{fieldName} must be between 0 and 100.");
        }

        private void ValidateNotNegative(Expression<Func<BranchSurveyForm, decimal?>> field, string fieldName)
        {
            RuleFor(field).GreaterThanOrEqualTo(0m).WithMessage($"{fieldName} cannot be negative.");
        }

        private void ValidateNotNegative(Expression<Func<BranchSurveyForm, int?>> field, string fieldName)
        {
            RuleFor(field).GreaterThanOrEqualTo(0).WithMessage($"{fieldName} cannot be negative.");
        }
    }
}
