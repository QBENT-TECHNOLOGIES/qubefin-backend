using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record SubmitInterviewAssessmentCommand(Guid PanelId, AssessmentSubmitDto Assessment, Guid SubmittedBy) : IRequest<Result>;

public class SubmitInterviewAssessmentCommandValidator : AbstractValidator<SubmitInterviewAssessmentCommand>
{
    public SubmitInterviewAssessmentCommandValidator()
    {
        RuleFor(x => x.PanelId).NotEmpty().WithMessage("Interview panel entry is required.");
        RuleFor(x => x.Assessment.IsRecommendedForPosition).NotNull().WithMessage("Recommendation (Yes/No) is required.");
        RuleFor(x => x.Assessment.AppearanceAttitudeRating).InclusiveBetween(0, 5).When(x => x.Assessment.AppearanceAttitudeRating.HasValue);
        RuleFor(x => x.Assessment.PersonalityRating).InclusiveBetween(0, 5).When(x => x.Assessment.PersonalityRating.HasValue);
        RuleFor(x => x.Assessment.CommunicationRating).InclusiveBetween(0, 5).When(x => x.Assessment.CommunicationRating.HasValue);
        RuleFor(x => x.Assessment.EducationRating).InclusiveBetween(0, 5).When(x => x.Assessment.EducationRating.HasValue);
        RuleFor(x => x.Assessment.WorkExperienceRating).InclusiveBetween(0, 5).When(x => x.Assessment.WorkExperienceRating.HasValue);
        RuleFor(x => x.Assessment.TechnicalCompetenceRating).InclusiveBetween(0, 5).When(x => x.Assessment.TechnicalCompetenceRating.HasValue);
        RuleFor(x => x.Assessment.FlexibilityRating).InclusiveBetween(0, 5).When(x => x.Assessment.FlexibilityRating.HasValue);
        RuleFor(x => x.Assessment.AmbitionRating).InclusiveBetween(0, 5).When(x => x.Assessment.AmbitionRating.HasValue);
        RuleFor(x => x.Assessment.PotentialRating).InclusiveBetween(0, 5).When(x => x.Assessment.PotentialRating.HasValue);
        RuleFor(x => x.Assessment.OthersRating).InclusiveBetween(0, 5).When(x => x.Assessment.OthersRating.HasValue);
    }
}

internal sealed class SubmitInterviewAssessmentCommandHandler(IInterviewPanelRepository panelRepository, IUnitOfWork unitOfWork) : IRequestHandler<SubmitInterviewAssessmentCommand, Result>
{
    public async Task<Result> Handle(SubmitInterviewAssessmentCommand request, CancellationToken cancellationToken)
    {
        if (request.SubmittedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByIdAsync(request.PanelId);
        if (panel is null)
        {
            return Result.Fail("Interview panel entry not found.");
        }

        var dto = request.Assessment;
        var details = new AssessmentDetails(
            dto.AppearanceAttitudeRating, dto.AppearanceAttitudeRemarks,
            dto.PersonalityRating, dto.PersonalityRemarks,
            dto.CommunicationRating, dto.CommunicationRemarks,
            dto.EducationRating, dto.EducationRemarks,
            dto.WorkExperienceRating, dto.WorkExperienceRemarks,
            dto.TechnicalCompetenceRating, dto.TechnicalCompetenceRemarks,
            dto.FlexibilityRating, dto.FlexibilityRemarks,
            dto.AmbitionRating, dto.AmbitionRemarks,
            dto.PotentialRating, dto.PotentialRemarks,
            dto.OthersRating, dto.OthersRemarks,
            dto.AnyOtherJobsSuitedRemarks,
            dto.IsRecommendedForPosition,
            dto.PositiveRemarks,
            dto.NegativeRemarks);

        var result = panel.SubmitAssessment(details, request.SubmittedBy);
        if (!result)
        {
            return Result.Fail("This assessment has already been submitted and cannot be changed.");
        }

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}