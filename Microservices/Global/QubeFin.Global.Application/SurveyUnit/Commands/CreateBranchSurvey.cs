using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Commands;

#region --- COMMAND ---
public record CreateBranchSurveyCommand(BranchSurveyRequest SurveyRequest, Guid CreatedBy) : IRequest<Result<CreateBranchSurveyResponse>>;
#endregion

#region --- VALIDATION ---
public class CreateBranchSurveyCommandValidator : AbstractValidator<CreateBranchSurveyCommand>
{
    public CreateBranchSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyRequest).NotNull().WithMessage("Survey request is required.");
        RuleFor(v => v.SurveyRequest.SurveyId).NotEmpty().WithMessage("Survey ID is required.");
        RuleFor(v => v.SurveyRequest.SurveyDate).NotNull().WithMessage("Survey date is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record CreateBranchSurveyResponse(bool Created, Guid Id);
#endregion

#region --- HANDLER ---
internal sealed class CreateBranchSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateBranchSurveyCommand, Result<CreateBranchSurveyResponse>>
{
    public async Task<Result<CreateBranchSurveyResponse>> Handle(CreateBranchSurveyCommand request, CancellationToken cancellationToken)
    {
        var branchSurvey = BranchSurvey.Create(
            Guid.NewGuid(),
            request.SurveyRequest.SurveyId,
            request.SurveyRequest.SurveyDate,
            new BranchSurveyGeographicInformation(
                    request.SurveyRequest.GeographicInformation.ProposedOperationalArea,
                    request.SurveyRequest.GeographicInformation.AdministrativeUnitId,
                    request.SurveyRequest.GeographicInformation.PinCode,
                    request.SurveyRequest.GeographicInformation.Latitude,
                    request.SurveyRequest.GeographicInformation.Longitude,
                    request.SurveyRequest.GeographicInformation.GeoTag,
                    request.SurveyRequest.GeographicInformation.NearestLandmark,
                    request.SurveyRequest.GeographicInformation.AdministrativeStatus,
                    request.SurveyRequest.GeographicInformation.DistanceFromExistingWeGrowBranch,
                    request.SurveyRequest.GeographicInformation.DistanceFromDistrictHeadquarters),
            new BranchSurveyAccessibilityAssessment(),
            new BranchSurveyDemographicProfile(),
            new BranchSurveyEconomicProfile(),
            new BranchSurveyMarketPotential(),
            new BranchSurveyTransportationFacilities(),
            new BranchSurveyFinancialInclusionStatus(),
            new BranchSurveyMicrofinanceCompetition(),
            new BranchSurveyBusinessPotential(),
            new BranchSurveyRiskAssessment(),
            new BranchSurveyComplianceVerification(),
            new BranchSurveyRecommendation(),
            request.CreatedBy
            );

        var saveResposne = await surveyRepository.AddBranchSurvey(branchSurvey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new CreateBranchSurveyResponse(true, saveResposne));
    }
}
#endregion