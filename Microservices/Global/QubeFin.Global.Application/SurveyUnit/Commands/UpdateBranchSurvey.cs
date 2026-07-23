using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Global.Application.SurveyUnit.Commands;

#region --- COMMAND ---
public record UpdateBranchSurveyCommand(BranchSurveyRequest SurveyRequest, Guid ModifiedBy) : IRequest<Result<UpdateBranchSurveyResponse>>;
#endregion

#region --- VALIDATION ---
public class UpdateBranchSurveyCommandValidator : AbstractValidator<UpdateBranchSurveyCommand>
{
    public UpdateBranchSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyRequest).NotNull().WithMessage("Survey request is required.");
        RuleFor(v => v.SurveyRequest.SurveyId).NotEmpty().WithMessage("Survey ID is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record UpdateBranchSurveyResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateBranchSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork, ISender sender) : IRequestHandler<UpdateBranchSurveyCommand, Result<UpdateBranchSurveyResponse>>
{
    public async Task<Result<UpdateBranchSurveyResponse>> Handle(UpdateBranchSurveyCommand request, CancellationToken cancellationToken)
    {
        var branchSurvey = await surveyRepository.GetBranchSurveyByIdAsync(request.SurveyRequest.Id);
        if (branchSurvey == null)
        {
            return Result.Fail(new Error($"Branch Survey with ID {request.SurveyRequest.Id} not found."));
        }
        if (request.SurveyRequest.GeographicInformation != null)
        {
            branchSurvey.UpdateGeographicInformation(
                new BranchSurveyGeographicInformation(
                    request.SurveyRequest.GeographicInformation.SurveyDate,
                    request.SurveyRequest.GeographicInformation.ProposedOperationalArea,
                    request.SurveyRequest.GeographicInformation.AdministrativeUnitId,
                    request.SurveyRequest.GeographicInformation.PinCode,
                    request.SurveyRequest.GeographicInformation.Latitude,
                    request.SurveyRequest.GeographicInformation.Longitude,
                    request.SurveyRequest.GeographicInformation.GeoTag,
                    request.SurveyRequest.GeographicInformation.NearestLandmark,
                    request.SurveyRequest.GeographicInformation.AdministrativeStatus,
                    request.SurveyRequest.GeographicInformation.DistanceFromExistingWeGrowBranch,
                    request.SurveyRequest.GeographicInformation.DistanceFromDistrictHeadquarters
                ),
                request.ModifiedBy
            );
        }
        if (request.SurveyRequest.AccessibilityAssessment != null)
        {
            branchSurvey.UpdateAccessibilityAssessment(
                new BranchSurveyAccessibilityAssessment(
                    request.SurveyRequest.AccessibilityAssessment.RoadCondition,
                    request.SurveyRequest.AccessibilityAssessment.PublicTransportAvailability,
                    request.SurveyRequest.AccessibilityAssessment.RailwayConnectivity,
                    request.SurveyRequest.AccessibilityAssessment.BusConnectivity,
                    request.SurveyRequest.AccessibilityAssessment.MobileNetworkCoverage,
                    request.SurveyRequest.AccessibilityAssessment.InternetAvailability,
                    request.SurveyRequest.AccessibilityAssessment.ElectricitySupply,
                    request.SurveyRequest.AccessibilityAssessment.DrinkingWaterAvailability,
                    request.SurveyRequest.AccessibilityAssessment.SafetyOfArea
                ),
                request.ModifiedBy
            );
        }
        if (request.SurveyRequest.DemographicProfile != null)
        {
            branchSurvey.UpdateDemographicProfile(
                new BranchSurveyDemographicProfile(
                    request.SurveyRequest.DemographicProfile.EstimatedPopulation,
                    request.SurveyRequest.DemographicProfile.NumberOfHouseholds,
                    request.SurveyRequest.DemographicProfile.AverageFamilySize,
                    request.SurveyRequest.DemographicProfile.FemalePopulationPercent,
                    request.SurveyRequest.DemographicProfile.LiteracyRate,
                    request.SurveyRequest.DemographicProfile.WorkingPopulation,
                    request.SurveyRequest.DemographicProfile.MinorityPopulationPercent,
                    request.SurveyRequest.DemographicProfile.ScheduledCastePercent,
                    request.SurveyRequest.DemographicProfile.ScheduledTribePercent,
                    request.SurveyRequest.DemographicProfile.MigrationTrend
                ),
                request.ModifiedBy
            );
        }
        if (request.SurveyRequest.EconomicProfile != null)
        {
            branchSurvey.UpdateEconomicProfile(new BranchSurveyEconomicProfile(
                request.SurveyRequest.EconomicProfile.AgriculturePercent,
                request.SurveyRequest.EconomicProfile.AgriculturalLabour,
                request.SurveyRequest.EconomicProfile.DairyLivestock,
                request.SurveyRequest.EconomicProfile.SmallBusiness,
                request.SurveyRequest.EconomicProfile.PettyTrade,
                request.SurveyRequest.EconomicProfile.CottageSmallIndustries,
                request.SurveyRequest.EconomicProfile.TransportActivities,
                request.SurveyRequest.EconomicProfile.ServiceHolders,
                request.SurveyRequest.EconomicProfile.DailyWageEarners,
                request.SurveyRequest.EconomicProfile.OtherIncomeGeneratingActivities,
                request.SurveyRequest.EconomicProfile.MainCrop,
                request.SurveyRequest.EconomicProfile.PeakBusinessSeason,
                request.SurveyRequest.EconomicProfile.LeanSeason,
                request.SurveyRequest.EconomicProfile.OverallEconomicCondition
            ), request.ModifiedBy);
        }

        await surveyRepository.UpdateBranchSurvey(branchSurvey);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateBranchSurveyResponse(true));
    }
}
#endregion