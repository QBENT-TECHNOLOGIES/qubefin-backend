using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Saves a panelist's in-progress assessment as a draft. Unlike submit, this does not lock the
/// record and does not require a recommendation yet. Saving a draft marks the panelist as attended.
/// Looked up by CandidateId + EmployeeId rather than a client-supplied PanelId - the endpoint overrides
/// EmployeeId from the authenticated user's claims, so a panelist can only ever save their own assessment.</summary>
public record SaveInterviewAssessmentDraftCommand(Guid CandidateId, Guid EmployeeId, AssessmentSubmitDto Assessment, Guid SavedBy) : IRequest<Result<string>>;

public class SaveInterviewAssessmentDraftCommandValidator : AbstractValidator<SaveInterviewAssessmentDraftCommand>
{
    public SaveInterviewAssessmentDraftCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Panelist is required.");
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

internal sealed class SaveInterviewAssessmentDraftCommandHandler(IInterviewPanelRepository panelRepository, IUnitOfWork unitOfWork) : IRequestHandler<SaveInterviewAssessmentDraftCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveInterviewAssessmentDraftCommand request, CancellationToken cancellationToken)
    {
        if (request.SavedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var panel = await panelRepository.GetByCandidateAndEmployeeAsync(request.CandidateId, request.EmployeeId);
        if (panel is null)
        {
            return new RecordNotFoundError("Interview panel entry not found for the given candidate and employee.");
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

        var result = panel.SaveAssessmentDraft(details, request.SavedBy);
        if (!result)
        {
            return new ValidationError("This assessment has already been submitted and cannot be changed.");
        }

        await panelRepository.UpdateAsync(panel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Assessment saved as draft successfully.");
    }
}
