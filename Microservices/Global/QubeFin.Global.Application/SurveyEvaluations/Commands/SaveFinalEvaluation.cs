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
public record SaveFinalEvaluationCommand(Guid SurveyId, EvaluationRequest Evaluation, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveFinalEvaluationCommandValidator : AbstractValidator<SaveFinalEvaluationCommand>
{
    public SaveFinalEvaluationCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Evaluation).NotNull().SetValidator(new EvaluationRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SaveFinalEvaluationCommandHandler(
    ISurveyRepository surveyRepository,
    IBranchSurveyRepository branchSurveyRepository,
    ISurveyEvaluationRepository evaluationRepository,
    ISurveyCommitteeRepository committeeRepository,
    INotificationRepository notificationRepository,
    ISurveyApprovalRepository approvalRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveFinalEvaluationCommand, Result<string>>
{
    // Notification messages are kept short.
    private const int MaxNotificationLength = 200;

    public async Task<Result<string>> Handle(SaveFinalEvaluationCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("Only Branch Surveys are evaluated here.");

        if (!SurveyEvaluationRules.IsOpenForEvaluation(survey.Status))
            return new ValidationError("This survey is not open for evaluation.");

        // Only the Committee Lead gives the final decision.
        if (!await committeeRepository.IsActiveLeadAsync(request.EmployeeId))
            return new ForbiddenError("Only the Committee Lead can give the final decision.");

        var activeEvaluations = await evaluationRepository.GetActiveBySurveyIdAsync(request.SurveyId);
        if (activeEvaluations.Any(e => e.IsFinal))
            return new ValidationError("The final decision has already been given.");

        // Every committee member must evaluate first.
        var requiredEmployeeIds = await committeeRepository.GetRequiredEvaluatorEmployeeIdsAsync();
        var doneEmployeeIds = activeEvaluations.Where(e => !e.IsFinal).Select(e => e.EmployeeId).ToHashSet();
        if (!requiredEmployeeIds.All(doneEmployeeIds.Contains))
            return new ValidationError("All committee members must evaluate the survey before the final decision.");

        var branchSurvey = await branchSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (branchSurvey is null)
            return new ValidationError("The survey form has not been filled in.");

        var decision = request.Evaluation.Decision;

        // An approved survey goes to the Managing Director, so someone must hold that post.
        var managingDirectorIds = new List<Guid>();
        if (decision == EvaluationDecisions.Approve)
        {
            managingDirectorIds = await approvalRepository.GetManagingDirectorEmployeeIdsAsync(survey.CompanyId);
            if (managingDirectorIds.Count == 0)
                return new ValidationError("No Managing Director is assigned, so the survey cannot be sent for approval.");
        }

        var comments = (request.Evaluation.Comments ?? string.Empty).Trim();
        var now = DateTime.Now;

        var finalEvaluation = new TblSurveyCommitteeEvaluation
        {
            Id = Guid.NewGuid(),
            SurveyId = request.SurveyId,
            EmployeeId = request.EmployeeId,
            Comments = comments,
            Status = decision,
            EvaluationDate = now,
            IsFinal = true,
            IsArchived = false
        };

        // The surveyors (and the person who created the survey) are told about Reject and Recheck.
        var surveyorEmployeeIds = survey.SurveyAssigneds.Select(a => a.EmployeeId).ToList();

        if (decision == EvaluationDecisions.Approve)
        {
            // Goes on to the Managing Director for approval.
            branchSurvey.IsCommiteeSubmit = true;
            branchSurvey.CommiteeSubmitOn = now;
            branchSurvey.LastModifiedBy = request.UserId;
            branchSurvey.LastModifiedOn = now;
            await evaluationRepository.AddAsync(finalEvaluation);
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.PendingApproval, request.UserId);

            await Notify(managingDirectorIds, "Branch Survey Waiting For Approval",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) has been approved by the committee and is waiting for your approval.",
                "clipboard-check", survey.Id, request.UserId);
        }
        else if (decision == EvaluationDecisions.Reject)
        {
            branchSurvey.IsRejected = true;
            branchSurvey.LastModifiedBy = request.UserId;
            branchSurvey.LastModifiedOn = now;
            await evaluationRepository.AddAsync(finalEvaluation);
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Rejected, request.UserId);

            var creatorEmployeeId = await surveyRepository.GetCreatorEmployeeIdAsync(request.SurveyId);
            if (creatorEmployeeId.HasValue)
                surveyorEmployeeIds.Add(creatorEmployeeId.Value);

            await Notify(surveyorEmployeeIds, "Branch Survey Rejected",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) was rejected. {comments}", "x-circle", survey.Id, request.UserId);
        }
        else
        {
            // Recheck: keep the old evaluations as history, put the survey back in Draft for the surveyors.
            await evaluationRepository.ArchiveActiveAsync(request.SurveyId);
            finalEvaluation.IsArchived = true;
            finalEvaluation.ArchivedOn = now;
            await evaluationRepository.AddAsync(finalEvaluation);

            branchSurvey.IsSurveyorSubmit = false;
            branchSurvey.IsCommiteeSubmit = false;
            branchSurvey.LastModifiedBy = request.UserId;
            branchSurvey.LastModifiedOn = now;
            await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Draft, request.UserId);

            await Notify(surveyorEmployeeIds, "Branch Survey Needs Recheck",
                $"{survey.AssignmentNo} ({survey.ProposedArea}) needs a recheck. {comments}", "rotate-ccw", survey.Id, request.UserId);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Final decision saved successfully.");
    }

    private async Task Notify(List<Guid> employeeIds, string title, string message, string icon, Guid surveyId, Guid userId)
    {
        if (message.Length > MaxNotificationLength)
            message = message.Substring(0, MaxNotificationLength - 3) + "...";

        await notificationRepository.AddForEmployeesAsync(
            employeeIds.Distinct().ToList(),
            title: title,
            message: message,
            icon: icon,
            referenceId: surveyId,
            referenceType: "SURVEY",
            actionUrl: "/secure/global/surveys",
            createdBy: userId);
    }
}
#endregion
