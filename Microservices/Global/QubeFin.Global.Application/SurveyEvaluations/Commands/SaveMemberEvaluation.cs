using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyEvaluations.Commands;

#region --- COMMAND ---
public record SaveMemberEvaluationCommand(Guid SurveyId, EvaluationRequest Evaluation, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveMemberEvaluationCommandValidator : AbstractValidator<SaveMemberEvaluationCommand>
{
    public SaveMemberEvaluationCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Evaluation).NotNull().SetValidator(new EvaluationRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SaveMemberEvaluationCommandHandler(
    ISurveyRepository surveyRepository,
    ISurveyEvaluationRepository evaluationRepository,
    ISurveyCommitteeRepository committeeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveMemberEvaluationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveMemberEvaluationCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("Only Branch Surveys are evaluated here.");

        if (!SurveyEvaluationRules.IsOpenForEvaluation(survey.Status))
            return new ValidationError("This survey is not open for evaluation.");

        // Only an active committee member can evaluate. The Committee Lead gives the final decision instead.
        if (!await committeeRepository.IsActiveEvaluatorAsync(request.EmployeeId))
            return new ForbiddenError("Only a Survey Committee member can evaluate the survey.");

        var activeEvaluations = await evaluationRepository.GetActiveBySurveyIdAsync(request.SurveyId);
        if (activeEvaluations.Any(e => e.IsFinal))
            return new ValidationError("The Committee Lead has already given the final decision.");

        var decision = request.Evaluation.Decision;
        var comments = (request.Evaluation.Comments ?? string.Empty).Trim();

        // The same member can change his evaluation until the Lead decides.
        var existing = await evaluationRepository.GetMemberEvaluationAsync(request.SurveyId, request.EmployeeId);
        if (existing is null)
        {
            await evaluationRepository.AddAsync(new TblSurveyCommitteeEvaluation
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                EmployeeId = request.EmployeeId,
                Comments = comments,
                Status = decision,
                EvaluationDate = DateTime.Now,
                IsFinal = false,
                IsArchived = false
            });
        }
        else
        {
            existing.Comments = comments;
            existing.Status = decision;
            existing.EvaluationDate = DateTime.Now;
        }

        // Once every required member has evaluated, the survey waits for the Lead.
        var requiredEmployeeIds = await committeeRepository.GetRequiredEvaluatorEmployeeIdsAsync();
        var doneEmployeeIds = activeEvaluations.Where(e => !e.IsFinal).Select(e => e.EmployeeId).ToHashSet();
        doneEmployeeIds.Add(request.EmployeeId);
        var allDone = requiredEmployeeIds.All(doneEmployeeIds.Contains);

        var newStatus = allDone ? SurveyStatuses.PendingLeadEvaluation : SurveyStatuses.EvaluationInProgress;
        if (survey.Status != newStatus)
            await surveyRepository.UpdateStatusAsync(request.SurveyId, newStatus, request.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Evaluation saved successfully.");
    }
}
#endregion
