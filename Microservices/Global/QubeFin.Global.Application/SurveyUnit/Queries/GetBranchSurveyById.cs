using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Queries;

#region --- QUERY ---
public record GetBranchSurveyById(Guid SurveyId, Guid EmployeeId) : IRequest<Result<GetBranchSurveyByIdResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetBranchSurveyByIdValidator : AbstractValidator<GetBranchSurveyById>
{
    public GetBranchSurveyByIdValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion
#region --- RESPONSE ---
public record GetBranchSurveyByIdResponse(BranchSurveyResponse? branchSurveyResponse);
#endregion

#region --- HANDLER ---
internal sealed class GetBranchSurveyByIdHandler(QubeFinDataContext context) : IRequestHandler<GetBranchSurveyById, Result<GetBranchSurveyByIdResponse>>
{
    public async Task<Result<GetBranchSurveyByIdResponse>> Handle(GetBranchSurveyById request, CancellationToken cancellationToken)
    {
        var surveyEntity = await context.TblSurveys.Include(m => m.TblSurveyAssigneds).Include(m => m.TblBranchSurveys).AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.SurveyId, cancellationToken: cancellationToken);
        if (surveyEntity is null)
        {
            return Result.Fail(new Error($"Survey with Id {request.SurveyId} not found."));
        }
        var branchSurveyEntity = surveyEntity?.TblBranchSurveys.FirstOrDefault();
        if (branchSurveyEntity is null)
        {
            return new GetBranchSurveyByIdResponse(new BranchSurveyResponse { IsSubmitButtonVisible = surveyEntity.TblSurveyAssigneds.Any(sa => sa.EmployeeId == request.EmployeeId && sa.IsLead) });
        }
        var users = await context.TblUsers.Where(u => u.Id == branchSurveyEntity.CreatedBy || u.Id == branchSurveyEntity.LastModifiedBy).AsNoTracking().ToListAsync(cancellationToken);
        return new GetBranchSurveyByIdResponse(new BranchSurveyResponse
        {
            Id = branchSurveyEntity.Id,
            SurveyId = branchSurveyEntity.SurveyId,
            SurveyDate = branchSurveyEntity.SurveyDate,
            ProposedOperationalArea = branchSurveyEntity.ProposedOperationalArea,
            AdministrativeUnitId = branchSurveyEntity.AdministrativeUnitId,
            PinCode = branchSurveyEntity.PinCode,
            Latitude = branchSurveyEntity.Latitude,
            Longitude = branchSurveyEntity.Longitude,
            GeoTag = branchSurveyEntity.GeoTag,
            NearestLandmark = branchSurveyEntity.NearestLandmark,
            AdministrativeStatus = branchSurveyEntity.AdministrativeStatus,
            DistanceFromExistingWeGrowBranch = branchSurveyEntity.DistanceFromExistingWeGrowBranch,
            DistanceFromDistrictHeadquarters = branchSurveyEntity.DistanceFromDistrictHeadquarters,
            RoadCondition = branchSurveyEntity.RoadCondition,
            PublicTransportAvailability = branchSurveyEntity.PublicTransportAvailability,
            RailwayConnectivity = branchSurveyEntity.RailwayConnectivity,
            BusConnectivity = branchSurveyEntity.BusConnectivity,
            MobileNetworkCoverage = branchSurveyEntity.MobileNetworkCoverage,
            InternetAvailability = branchSurveyEntity.InternetAvailability,
            ElectricitySupply = branchSurveyEntity.ElectricitySupply,
            DrinkingWaterAvailability = branchSurveyEntity.DrinkingWaterAvailability,
            SafetyOfArea = branchSurveyEntity.SafetyOfArea,
            EstimatedPopulation = branchSurveyEntity.EstimatedPopulation,
            NumberOfHouseholds = branchSurveyEntity.NumberOfHouseholds,
            AverageFamilySize = branchSurveyEntity.AverageFamilySize,
            FemalePopulationPercent = branchSurveyEntity.FemalePopulationPercent,
            LiteracyRate = branchSurveyEntity.LiteracyRate,
            WorkingPopulation = branchSurveyEntity.WorkingPopulation,
            MinorityPopulationPercent = branchSurveyEntity.MinorityPopulationPercent,
            ScheduledCastePercent = branchSurveyEntity.ScheduledCastePercent,
            ScheduledTribePercent = branchSurveyEntity.ScheduledTribePercent,
            MigrationTrend = branchSurveyEntity.MigrationTrend,
            AgriculturePercent = branchSurveyEntity.AgriculturePercent,
            AgriculturalLabour = branchSurveyEntity.AgriculturalLabour,
            DairyLivestock = branchSurveyEntity.DairyLivestock,
            SmallBusiness = branchSurveyEntity.SmallBusiness,
            PettyTrade = branchSurveyEntity.PettyTrade,
            CottageSmallIndustries = branchSurveyEntity.CottageSmallIndustries,
            TransportActivities = branchSurveyEntity.TransportActivities,
            ServiceHolders = branchSurveyEntity.ServiceHolders,
            DailyWageEarners = branchSurveyEntity.DailyWageEarners,
            OtherIncomeGeneratingActivities = branchSurveyEntity.OtherIncomeGeneratingActivities,
            MainCrop = branchSurveyEntity.MainCrop,
            PeakBusinessSeason = branchSurveyEntity.PeakBusinessSeason,
            LeanSeason = branchSurveyEntity.LeanSeason,
            OverallEconomicCondition = branchSurveyEntity.OverallEconomicCondition,
            EligibleHouseholds = branchSurveyEntity.EligibleHouseholds,
            PotentialWomenBorrowers = branchSurveyEntity.PotentialWomenBorrowers,
            Jlgpotential = branchSurveyEntity.Jlgpotential,
            IndividualBusinessLoansExpected = branchSurveyEntity.IndividualBusinessLoansExpected,
            PortfolioYear1 = branchSurveyEntity.PortfolioYear1,
            PortfolioYear2 = branchSurveyEntity.PortfolioYear2,
            PortfolioYear3 = branchSurveyEntity.PortfolioYear3,
            RailConnectivity = branchSurveyEntity.RailConnectivity,
            BusConnectivityAvailable = branchSurveyEntity.BusConnectivityAvailable,
            AutoTotoAvailability = branchSurveyEntity.AutoTotoAvailability,
            RoadAccessibility = branchSurveyEntity.RoadAccessibility,
            AccessibilityByMotorCycle = branchSurveyEntity.AccessibilityByMotorCycle,
            NumberOfBanks = branchSurveyEntity.NumberOfBanks,
            NumberOfRegionalRuralBanks = branchSurveyEntity.NumberOfRegionalRuralBanks,
            NumberOfCooperativeBanks = branchSurveyEntity.NumberOfCooperativeBanks,
            BankingCorrespondents = branchSurveyEntity.BankingCorrespondents,
            Atms = branchSurveyEntity.Atms,
            DigitalPaymentAcceptance = branchSurveyEntity.DigitalPaymentAcceptance,
            NameOfInstitution = branchSurveyEntity.NameOfInstitution,
            ApproxClients = branchSurveyEntity.ApproxClients,
            ApproxPortfolio = branchSurveyEntity.ApproxPortfolio,
            Parpercent = branchSurveyEntity.Parpercent,
            EstimatedEligibleHouseholds = branchSurveyEntity.EstimatedEligibleHouseholds,
            EstimatedWomenBorrowers = branchSurveyEntity.EstimatedWomenBorrowers,
            EstimatedNumberOfJlgsCentres = branchSurveyEntity.EstimatedNumberOfJlgsCentres,
            EstimatedLoanPortfolioPotential = branchSurveyEntity.EstimatedLoanPortfolioPotential,
            ExpectedMonthlyDisbursement = branchSurveyEntity.ExpectedMonthlyDisbursement,
            EstimatedCollectionEfficiency = branchSurveyEntity.EstimatedCollectionEfficiency,
            FloodRisk = branchSurveyEntity.FloodRisk,
            CycloneRisk = branchSurveyEntity.CycloneRisk,
            LandslideRisk = branchSurveyEntity.LandslideRisk,
            DroughtRisk = branchSurveyEntity.DroughtRisk,
            PoliticalDisturbanceRisk = branchSurveyEntity.PoliticalDisturbanceRisk,
            CommunalIssuesRisk = branchSurveyEntity.CommunalIssuesRisk,
            MigrationRisk = branchSurveyEntity.MigrationRisk,
            BusinessRisk = branchSurveyEntity.BusinessRisk,
            MultipleLendingRisk = branchSurveyEntity.MultipleLendingRisk,
            CollectionRisk = branchSurveyEntity.CollectionRisk,
            FraudRisk = branchSurveyEntity.FraudRisk,
            CompetitionRisk = branchSurveyEntity.CompetitionRisk,
            AreaVisitedPhysically = branchSurveyEntity.AreaVisitedPhysically,
            Gpsverified = branchSurveyEntity.Gpsverified,
            LocalReferencesVerified = branchSurveyEntity.LocalReferencesVerified,
            ExistingCustomersContacted = branchSurveyEntity.ExistingCustomersContacted,
            CompetitorVerificationCompleted = branchSurveyEntity.CompetitorVerificationCompleted,
            PhotographsAttached = branchSurveyEntity.PhotographsAttached,
            Recommendation = branchSurveyEntity.Recommendation,
            IsSurveyorSubmit = branchSurveyEntity.IsSurveyorSubmit,
            SurveyorSubmitOn = branchSurveyEntity.SurveyorSubmitOn,
            IsCommiteeSubmit = branchSurveyEntity.IsCommiteeSubmit,
            CommiteeSubmitOn = branchSurveyEntity.CommiteeSubmitOn,
            IsApproved = branchSurveyEntity.IsApproved,
            IsRejected = branchSurveyEntity.IsRejected,
            IsSubmitButtonVisible = (!branchSurveyEntity.IsSurveyorSubmit && branchSurveyEntity.Survey.TblSurveyAssigneds.Any(sa => sa.EmployeeId == request.EmployeeId && sa.IsLead)),

            AuditInfo = new AuditInfo
            {
                CreatedBy = users.FirstOrDefault(u => u.Id == branchSurveyEntity.CreatedBy)?.UserName ?? string.Empty,
                CreatedOn = branchSurveyEntity.CreatedOn,
                LastModifiedBy = users.FirstOrDefault(u => u.Id == branchSurveyEntity.LastModifiedBy)?.UserName ?? string.Empty,
                LastModifiedOn = branchSurveyEntity.LastModifiedOn
            },
        });
    }
}
#endregion

