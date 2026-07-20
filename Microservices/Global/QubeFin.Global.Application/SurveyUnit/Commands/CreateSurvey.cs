using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Commands;


#region --- COMMAND ---
public record CreateSurveyCommand(SurveyRequest SurveyRequest) : IRequest<Result<CreateSurveyResponse>>;
#endregion

#region --- VALIDATION ---
public class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyRequest).NotNull().WithMessage("Survey request is required.");
        RuleFor(v => v.SurveyRequest.SurveyType).NotEmpty().WithMessage("Survey type is required.");
        RuleFor(v => v.SurveyRequest.AssignmentNo).NotEmpty().WithMessage("Assignment number is required.");
        RuleFor(v => v.SurveyRequest.AssignmentDate).NotEmpty().WithMessage("Assignment date is required.");
        RuleFor(v => v.SurveyRequest.ProposedArea).NotEmpty().WithMessage("Proposed area is required.");
        RuleFor(v => v.SurveyRequest.AdministrativeUnitId).NotEmpty().WithMessage("Administrative unit ID is required.");
        RuleFor(v => v.SurveyRequest.TentativeSubmissionDate).NotEmpty().WithMessage("Tentative submission date is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record CreateSurveyResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class CreateSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateSurveyCommand, Result<CreateSurveyResponse>>
{
    public async Task<Result<CreateSurveyResponse>> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = Survey.Create(
            Guid.NewGuid(),
            request.SurveyRequest.SurveyType,
            request.SurveyRequest.AssignmentNo,
            request.SurveyRequest.AssignmentDate,
            request.SurveyRequest.ProposedArea,
            request.SurveyRequest.AdministrativeUnitId,
            request.SurveyRequest.TentativeSubmissionDate,
            request.SurveyRequest.CreatedBy
        );
        await surveyRepository.AddSurvey(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new CreateSurveyResponse(true));
    }
}
#endregion

