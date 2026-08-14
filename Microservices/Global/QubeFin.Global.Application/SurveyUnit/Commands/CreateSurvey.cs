using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Commands;


#region --- COMMAND ---
public record CreateSurveyCommand(SurveyRequest SurveyRequest, Guid userId) : IRequest<Result<bool>>;
#endregion

#region --- VALIDATION ---
public class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyRequest).NotNull().WithMessage("Survey request is required.");
        RuleFor(v => v.SurveyRequest.SurveyType).NotEmpty().WithMessage("Survey type is required.");
        RuleFor(v => v.SurveyRequest.AssignmentDate).NotEmpty().WithMessage("Assignment date is required.");
        RuleFor(v => v.SurveyRequest.ProposedArea).NotEmpty().WithMessage("Proposed area is required.");
        RuleFor(v => v.SurveyRequest.AdministrativeUnitId).NotEmpty().WithMessage("Administrative unit ID is required.");
        RuleFor(v => v.SurveyRequest.TentativeSubmissionDate).NotEmpty().WithMessage("Tentative submission date is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CreateSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateSurveyCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var surveyAssigneds = request.SurveyRequest.SurveyAssigneds
            .Select(x => SurveyAssigned.Create(
                Guid.Empty,
                x.EmployeeId,
                x.IsLead,
                request.userId))
            .ToList();


        var survey = Survey.Create(
            Guid.NewGuid(),
            request.SurveyRequest.SurveyType,
            request.SurveyRequest.AssignmentDate,
            request.SurveyRequest.ProposedArea,
            request.SurveyRequest.AdministrativeUnitId,
            request.SurveyRequest.TentativeSubmissionDate,
            surveyAssigneds,
            request.userId
        );
        await surveyRepository.AddSurvey(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(true);
    }
}
#endregion

