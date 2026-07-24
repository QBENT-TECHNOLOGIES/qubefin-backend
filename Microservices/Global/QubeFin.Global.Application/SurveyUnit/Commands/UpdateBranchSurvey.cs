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
public record UpdateBranchSurveyResponse(bool Created, string Message);
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
        string message = "";
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
            message = "Branch survey geographic information updated successfully.";
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
            message = "Branch survey accessibility assessment updated successfully.";
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
            message = "Branch survey demographic profile updated successfully.";
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
            message = "Branch survey economic profile updated successfully.";
        }
        if (request.SurveyRequest.MarketPotential != null)
        {
            branchSurvey.UpdateMarketPotential(new BranchSurveyMarketPotential(
                 request.SurveyRequest.MarketPotential.EligibleHouseholds,
            request.SurveyRequest.MarketPotential.PotentialWomenBorrowers,
            request.SurveyRequest.MarketPotential.Jlgpotential,
            request.SurveyRequest.MarketPotential.IndividualBusinessLoansExpected,
            request.SurveyRequest.MarketPotential.PortfolioYear1,
            request.SurveyRequest.MarketPotential.PortfolioYear2,
            request.SurveyRequest.MarketPotential.PortfolioYear3
            ), request.ModifiedBy);
            message = "Branch survey market potential updated successfully.";
        }
        if (request.SurveyRequest.TransportationFacilities != null)
        {
            branchSurvey.UpdateTransportationFacilities(new BranchSurveyTransportationFacilities(
                request.SurveyRequest.TransportationFacilities.RailConnectivity,
                request.SurveyRequest.TransportationFacilities.BusConnectivityAvailable,
                request.SurveyRequest.TransportationFacilities.AutoTotoAvailability,
                request.SurveyRequest.TransportationFacilities.RoadAccessibility,
                request.SurveyRequest.TransportationFacilities.AccessibilityByMotorCycle
            ), request.ModifiedBy);
            message = "Branch survey transportation facilities updated successfully.";
        }
        if (request.SurveyRequest.FinancialInclusionStatus != null)
        {
            branchSurvey.UpdateFinancialInclusionStatus(new BranchSurveyFinancialInclusionStatus()
            {
                NumberOfBanks = request.SurveyRequest.FinancialInclusionStatus.NumberOfBanks,
                NumberOfRegionalRuralBanks = request.SurveyRequest.FinancialInclusionStatus.NumberOfRegionalRuralBanks,
                NumberOfCooperativeBanks = request.SurveyRequest.FinancialInclusionStatus.NumberOfCooperativeBanks,
                BankingCorrespondents = request.SurveyRequest.FinancialInclusionStatus.BankingCorrespondents,
                Atms = request.SurveyRequest.FinancialInclusionStatus.Atms,
                DigitalPaymentAcceptance = request.SurveyRequest.FinancialInclusionStatus.DigitalPaymentAcceptance
            }, request.ModifiedBy);
            message = "Branch survey financial inclusion status updated successfully.";
        }
        if (request.SurveyRequest.MicrofinanceCompetition != null)
        {
            branchSurvey.UpdateMicrofinanceCompetition(new BranchSurveyMicrofinanceCompetition()
            {
                NameOfInstitution = request.SurveyRequest.MicrofinanceCompetition.NameOfInstitution,
                ApproxClients = request.SurveyRequest.MicrofinanceCompetition.ApproxClients,
                ApproxPortfolio = request.SurveyRequest.MicrofinanceCompetition.ApproxPortfolio,
                Parpercent = request.SurveyRequest.MicrofinanceCompetition.Parpercent
            }, request.ModifiedBy);
            message = "Branch survey microfinance competition updated successfully.";
        }
        if (request.SurveyRequest.BusinessPotential != null)
        {
            branchSurvey.UpdateBusinessPotential(new BranchSurveyBusinessPotential(
                request.SurveyRequest.BusinessPotential.EstimatedEligibleHouseholds,
                request.SurveyRequest.BusinessPotential.EstimatedWomenBorrowers,
                request.SurveyRequest.BusinessPotential.EstimatedNumberOfJlgsCentres,
                request.SurveyRequest.BusinessPotential.EstimatedLoanPortfolioPotential,
                request.SurveyRequest.BusinessPotential.ExpectedMonthlyDisbursement,
                request.SurveyRequest.BusinessPotential.EstimatedCollectionEfficiency
            ), request.ModifiedBy);
            message = "Branch survey business potential updated successfully.";
        }
        if (request.SurveyRequest.RiskAssessment != null)
        {
            branchSurvey.UpdateRiskAssessment(new BranchSurveyRiskAssessment(
                request.SurveyRequest.RiskAssessment.FloodRisk,
                request.SurveyRequest.RiskAssessment.CycloneRisk,
                request.SurveyRequest.RiskAssessment.LandslideRisk,
                request.SurveyRequest.RiskAssessment.DroughtRisk,
                request.SurveyRequest.RiskAssessment.PoliticalDisturbanceRisk,
                request.SurveyRequest.RiskAssessment.CommunalIssuesRisk,
                request.SurveyRequest.RiskAssessment.MigrationRisk,
                request.SurveyRequest.RiskAssessment.BusinessRisk,
                request.SurveyRequest.RiskAssessment.MultipleLendingRisk,
                request.SurveyRequest.RiskAssessment.CollectionRisk,
                request.SurveyRequest.RiskAssessment.FraudRisk,
                request.SurveyRequest.RiskAssessment.CompetitionRisk
            ), request.ModifiedBy);
            message = "Branch survey risk assessment updated successfully.";
        }
        if (request.SurveyRequest.ComplianceVerification != null)
        {
            branchSurvey.UpdateComplianceVerification(new BranchSurveyComplianceVerification(
                request.SurveyRequest.ComplianceVerification.AreaVisitedPhysically,
                request.SurveyRequest.ComplianceVerification.Gpsverified,
                request.SurveyRequest.ComplianceVerification.LocalReferencesVerified,
                request.SurveyRequest.ComplianceVerification.ExistingCustomersContacted,
                request.SurveyRequest.ComplianceVerification.CompetitorVerificationCompleted,
                request.SurveyRequest.ComplianceVerification.PhotographsAttached
            ), request.ModifiedBy);
            message = "Branch survey compliance verification updated successfully.";
        }
        if (request.SurveyRequest.Recommendation != null)
        {
            branchSurvey.UpdateRecommendation(new BranchSurveyRecommendation(
                request.SurveyRequest.Recommendation.Recommendation
            ), request.ModifiedBy);
            message = "Branch survey recommendation updated successfully.";
        }

        await surveyRepository.UpdateBranchSurvey(branchSurvey);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateBranchSurveyResponse(true, message));
    }
}
#endregion