using FluentValidation;
using System.Linq.Expressions;

namespace QubeFin.Global.Application.GroupSurveys.Models
{
    // Checks that only need the form itself. A draft needs only the Survey Date; nothing else is mandatory.
    public class GroupSurveyFormValidator : AbstractValidator<GroupSurveyForm>
    {
        public GroupSurveyFormValidator()
        {
            RuleFor(f => f.SurveyDate).NotEmpty().WithMessage("Date of survey is required.");

            // Geographic
            RuleFor(f => f.Geographic.NearestLandmark).MaximumLength(200).WithMessage("Nearest landmark can have at most 200 characters.");
            RuleFor(f => f.Geographic.Latitude).InclusiveBetween(-90m, 90m).When(f => f.Geographic.Latitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90.");
            RuleFor(f => f.Geographic.Longitude).InclusiveBetween(-180m, 180m).When(f => f.Geographic.Longitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180.");
            // The location is saved as one point, so latitude and longitude go together.
            RuleFor(f => f.Geographic)
                .Must(g => g.Latitude.HasValue == g.Longitude.HasValue)
                .WithMessage("Enter both latitude and longitude, or leave both empty.");

            // Profile
            ValidateNotNegative(f => f.Profile.ApproximateHouseholds, "Approximate number of households");
            ValidateNotNegative(f => f.Profile.EstimatedPopulation, "Estimated population");
            ValidateOption(f => f.Profile.MainOccupation, GroupSurveyOptions.MainOccupation, "Main occupation");
            RuleFor(f => f.Profile.MainOccupationOther).MaximumLength(100).WithMessage("Other occupation can have at most 100 characters.");

            // Environment
            ValidateOption(f => f.Environment.RoadAccessibility, GroupSurveyOptions.YesNo, "Road accessibility");
            ValidateOption(f => f.Environment.AutoTotoAvailability, GroupSurveyOptions.YesNo, "Auto / Toto availability");
            ValidateOption(f => f.Environment.CommunityHarmony, GroupSurveyOptions.GoodModeratePoor, "Community harmony");
            ValidateOption(f => f.Environment.WomenEconomicParticipation, GroupSurveyOptions.GoodModeratePoor, "Participation of women in economic activities");
            ValidateOption(f => f.Environment.SocialAcceptanceOfMicrofinance, GroupSurveyOptions.GoodModeratePoor, "Social acceptance of microfinance");
            ValidateOption(f => f.Environment.PoliticalStability, GroupSurveyOptions.StableModerateSensitive, "Political stability");
            ValidateOption(f => f.Environment.PoliticalDisturbance, GroupSurveyOptions.StableModerateSensitive, "Any political disturbance");
            ValidateOption(f => f.Environment.LocalInfluenceOnLending, GroupSurveyOptions.StableModerateSensitive, "Local influence on lending activities");
            ValidateOption(f => f.Environment.ElectionRelatedIssues, GroupSurveyOptions.StableModerateSensitive, "Election-related issues");

            // Risk
            ValidateOption(f => f.Risk.FloodProne, GroupSurveyOptions.YesNo, "Flood prone area");
            ValidateOption(f => f.Risk.WaterLogging, GroupSurveyOptions.YesNo, "Water logging");
            ValidateOption(f => f.Risk.DroughtProne, GroupSurveyOptions.YesNo, "Drought prone");
            ValidateOption(f => f.Risk.FlashFloodRisk, GroupSurveyOptions.YesNo, "Flash flood risk");
            ValidateOption(f => f.Risk.CycloneRisk, GroupSurveyOptions.YesNo, "Cyclone risk");
            ValidateOption(f => f.Risk.RiverErosion, GroupSurveyOptions.YesNo, "River erosion");
            ValidateOption(f => f.Risk.OtherNaturalRisks, GroupSurveyOptions.YesNo, "Other natural risks");
            ValidateNotNegative(f => f.Risk.NumberOfMfis, "Number of MFIs operating");
            ValidateNotNegative(f => f.Risk.SmallFinanceBanks, "Small finance banks");
            ValidateNotNegative(f => f.Risk.ShgFinancingInstitutions, "SHG financing institutions");

            // Market
            ValidateNotNegative(f => f.Market.DistanceFromBranchKm, "Distance from the branch");
            ValidateNotNegative(f => f.Market.DistanceFromNearestWeGrowCentreKm, "Distance from nearest existing WeGrow centre");
            ValidateNotNegative(f => f.Market.DistanceFromNearestCompetingCentreKm, "Distance from nearest competing centre");
            ValidateNotNegative(f => f.Market.AverageTravelTimeMinutes, "Average travel time");
            ValidateNotNegative(f => f.Market.GroupsThatCanBeFormed, "Number of groups that can be formed");
            ValidateNotNegative(f => f.Market.EstimatedWomenBorrowers, "Estimated women borrowers");

            // Observation
            ValidateOption(f => f.Observation.SuitableMeetingPlaces, GroupSurveyOptions.YesNo, "Availability of suitable centre meeting places");
            ValidateOption(f => f.Observation.CommunityResponseTowardsWeGrow, GroupSurveyOptions.GoodModeratePoor, "Community response towards WeGrow");
            ValidateOption(f => f.Observation.SurveyorRecommendation, GroupSurveyOptions.Recommendation, "Recommendation");
            RuleFor(f => f.Observation.OtherObservations).MaximumLength(1000).WithMessage("Other observations can have at most 1000 characters.");
            RuleFor(f => f.Observation.SurveyorComments).MaximumLength(1000).WithMessage("Comments can have at most 1000 characters.");
        }

        private void ValidateOption(Expression<Func<GroupSurveyForm, string?>> field, string[] allowedValues, string fieldName)
        {
            RuleFor(field)
                .Must(value => string.IsNullOrEmpty(value) || allowedValues.Contains(value))
                .WithMessage($"{fieldName} must be one of: {string.Join(", ", allowedValues)}.");
        }

        // An empty (null) value is always accepted: a draft does not need every field.
        private void ValidateNotNegative(Expression<Func<GroupSurveyForm, decimal?>> field, string fieldName)
        {
            RuleFor(field).GreaterThanOrEqualTo(0m).WithMessage($"{fieldName} cannot be negative.");
        }

        private void ValidateNotNegative(Expression<Func<GroupSurveyForm, int?>> field, string fieldName)
        {
            RuleFor(field).GreaterThanOrEqualTo(0).WithMessage($"{fieldName} cannot be negative.");
        }
    }
}
