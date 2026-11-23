using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.BranchSurveys
{
    // Checks that the Branch Survey form is complete before the Lead Surveyor submits it.
    // Every field is required EXCEPT: Other Income Generating Activities, Main Crop, Peak Business Season,
    // Lean Season and the competitor rows. (The Geo Tag is filled in automatically from the location.)
    public static class BranchSurveySubmitCheck
    {
        // Returns the names of the required fields that are still empty.
        public static List<string> GetMissingFields(TblBranchSurvey b)
        {
            var requiredFields = new (string Label, object? Value)[]
            {
                // A. Geographic Information
                ("Proposed Operational Area", b.ProposedOperationalArea),
                ("Police Station", b.PoliceStation),
                ("PIN Code", b.PinCode),
                ("Latitude and Longitude", b.Location),
                ("Nearest Landmark", b.NearestLandmark),
                ("Administrative Status", b.AdministrativeStatus),
                ("Distance from Existing WeGrow Branch", b.DistanceFromExistingWeGrowBranch),
                ("Distance from District Headquarters", b.DistanceFromDistrictHeadquarters),
                // B. Accessibility Assessment
                ("Road Condition", b.RoadCondition),
                ("Public Transport Availability", b.PublicTransportAvailability),
                ("Railway Connectivity", b.RailwayConnectivity),
                ("Bus Connectivity", b.BusConnectivity),
                ("Mobile Network Coverage", b.MobileNetworkCoverage),
                ("Internet Availability", b.InternetAvailability),
                ("Electricity Supply", b.ElectricitySupply),
                ("Drinking Water Availability", b.DrinkingWaterAvailability),
                ("Safety of Area", b.SafetyOfArea),
                // C. Demographic Profile
                ("Estimated Population", b.EstimatedPopulation),
                ("Number of Households", b.NumberOfHouseholds),
                ("Average Family Size", b.AverageFamilySize),
                ("Female Population", b.FemalePopulationPercent),
                ("Literacy Rate", b.LiteracyRate),
                ("Working Population", b.WorkingPopulation),
                ("Minority Population", b.MinorityPopulationPercent),
                ("Scheduled Caste", b.ScheduledCastePercent),
                ("Scheduled Tribe", b.ScheduledTribePercent),
                ("Migration Trend", b.MigrationTrend),
                // D. Economic Profile
                ("Agriculture", b.AgriculturePercent),
                ("Agricultural Labour", b.AgriculturalLabour),
                ("Dairy / Livestock", b.DairyLivestock),
                ("Small Business", b.SmallBusiness),
                ("Petty Trade", b.PettyTrade),
                ("Cottage / Small Industries", b.CottageSmallIndustries),
                ("Transport Activities", b.TransportActivities),
                ("Service Holders", b.ServiceHolders),
                ("Daily Wage Earners", b.DailyWageEarners),
                ("Overall Economic Condition", b.OverallEconomicCondition),
                // E. Market Potential Assessment
                ("Eligible Households", b.EligibleHouseholds),
                ("Potential Women Borrowers", b.PotentialWomenBorrowers),
                ("JLG Potential", b.Jlgpotential),
                ("Individual Business Loans", b.IndividualBusinessLoansExpected),
                ("Expected Portfolio - Year 1", b.PortfolioYear1),
                ("Expected Portfolio - Year 2", b.PortfolioYear2),
                ("Expected Portfolio - Year 3", b.PortfolioYear3),
                // F. Transportation Facilities
                ("Rail Connectivity (Transportation)", b.RailConnectivity),
                ("Bus Connectivity (Transportation)", b.BusConnectivityAvailable),
                ("Auto / Toto Availability", b.AutoTotoAvailability),
                ("Road Accessibility", b.RoadAccessibility),
                ("Motorcycle Accessibility", b.AccessibilityByMotorCycle),
                // G. Financial Inclusion Status
                ("Number of Banks", b.NumberOfBanks),
                ("Number of Regional Rural Banks", b.NumberOfRegionalRuralBanks),
                ("Number of Cooperative Banks", b.NumberOfCooperativeBanks),
                ("Banking Correspondents", b.BankingCorrespondents),
                ("ATMs", b.Atms),
                ("Digital Payment Acceptance", b.DigitalPaymentAcceptance),
                // I. Business Potential Assessment
                ("Estimated Eligible Households", b.EstimatedEligibleHouseholds),
                ("Estimated Women Borrowers", b.EstimatedWomenBorrowers),
                ("Estimated Number of JLG Centres", b.EstimatedNumberOfJlgsCentres),
                ("Estimated Loan Portfolio Potential", b.EstimatedLoanPortfolioPotential),
                ("Expected Monthly Disbursement", b.ExpectedMonthlyDisbursement),
                ("Estimated Collection Efficiency", b.EstimatedCollectionEfficiency),
                // J. Risk Assessment
                ("Flood Risk", b.FloodRisk),
                ("Cyclone Risk", b.CycloneRisk),
                ("Landslide Risk", b.LandslideRisk),
                ("Drought Risk", b.DroughtRisk),
                ("Political Disturbance Risk", b.PoliticalDisturbanceRisk),
                ("Communal Issues Risk", b.CommunalIssuesRisk),
                ("Migration Risk", b.MigrationRisk),
                ("Multiple Lending Risk", b.MultipleLendingRisk),
                ("Collection Risk", b.CollectionRisk),
                ("Fraud Risk", b.FraudRisk),
                ("Competition Risk", b.CompetitionRisk),
                // K. Compliance Verification
                ("Area Visited Physically", b.AreaVisitedPhysically),
                ("GPS Verified", b.Gpsverified),
                ("Local References Verified", b.LocalReferencesVerified),
                ("Existing Customers Contacted", b.ExistingCustomersContacted),
                ("Competitor Verification Completed", b.CompetitorVerificationCompleted),
                ("Photographs Attached", b.PhotographsAttached),
                // L and M
                ("Recommendation", b.Recommendation),
                ("Comments", b.SurveyorComments),
            };

            return requiredFields
                .Where(field => IsEmpty(field.Value))
                .Select(field => field.Label)
                .ToList();
        }

        // A number is only empty when it is null. 0 is a real answer.
        private static bool IsEmpty(object? value)
        {
            if (value is null) return true;
            if (value is string text) return string.IsNullOrWhiteSpace(text);
            return false;
        }
    }
}
