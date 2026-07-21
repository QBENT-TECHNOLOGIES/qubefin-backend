using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyCommittees.Commands;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Commands;


#region --- COMMAND ---
public record UpdateSurveyCommand(SurveyRequest SurveyRequest, Guid userId) : IRequest<Result<UpdateSurveyResponse>>;
#endregion

#region --- VALIDATION ---
public class UpdateSurveyCommandValidator : AbstractValidator<UpdateSurveyCommand>
{
    public UpdateSurveyCommandValidator()
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

#region --- RESPONSE ---
public record UpdateSurveyResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSurveyCommand, Result<UpdateSurveyResponse>>
{
    public async Task<Result<UpdateSurveyResponse>> Handle(UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        var surveyEntity = await surveyRepository.GetByIdAsync(request.SurveyRequest.Id);
        if (surveyEntity is null) return new RecordNotFoundError("Member not found");

        var surveyAssigneds = request.SurveyRequest.SurveyAssigneds
            .Select(x => SurveyAssigned.Create(
                surveyEntity.Id,
                x.EmployeeId,
                x.IsLead,
                request.userId))
            .ToList();

        surveyEntity.Update(
            surveyType: request.SurveyRequest.SurveyType,
            assignmentDate: request.SurveyRequest.AssignmentDate,
            proposedArea: request.SurveyRequest.ProposedArea,
            administrativeUnitId: request.SurveyRequest.AdministrativeUnitId,
            tentativeSubmissionDate: request.SurveyRequest.TentativeSubmissionDate,
            surveyAssigneds,
            lastModifiedBy: request.userId
        );

        await surveyRepository.UpdateSurvey(surveyEntity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new UpdateSurveyResponse(true));
    }
}
#endregion