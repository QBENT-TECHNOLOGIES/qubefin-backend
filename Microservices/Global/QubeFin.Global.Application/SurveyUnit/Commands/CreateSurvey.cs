using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Commands;


#region --- COMMAND ---
public record CreateSurveyCommand(SurveyRequest SurveyRequest, Guid userId, Guid employeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyRequest).NotNull().WithMessage("Survey request is required.");
        RuleFor(v => v.SurveyRequest).SetValidator(new SurveyRequestValidator()).When(v => v.SurveyRequest != null);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CreateSurveyCommandHandler(
    ISurveyRepository surveyRepository,
    ISurveyCommitteeRepository surveyCommitteeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateSurveyCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var surveyRequest = request.SurveyRequest;

        // Group Survey assignment (done by the Branch Manager) is built in a later phase.
        if (surveyRequest.SurveyType == SurveyTypes.Group)
            return new ValidationError("Group Survey assignment is not available yet.");

        // Only the Committee Lead can assign a Branch Survey.
        var isCommitteeLead = await surveyCommitteeRepository.IsActiveLeadAsync(request.employeeId);
        if (!isCommitteeLead)
            return new ForbiddenError("Only the Committee Lead can create a Branch Survey.");

        // A Survey Committee member cannot also be a surveyor.
        var assignedEmployeeIds = surveyRequest.SurveyAssigneds.Select(x => x.EmployeeId).ToList();
        if (await surveyCommitteeRepository.AnyActiveMemberAsync(assignedEmployeeIds))
            return new ValidationError("A Survey Committee member cannot be assigned as a surveyor.");

        // When a rejected survey is created again, the earlier survey must really be a rejected one.
        if (surveyRequest.PreviousSurveyId.HasValue)
        {
            var previousSurvey = await surveyRepository.GetByIdAsync(surveyRequest.PreviousSurveyId.Value);
            if (previousSurvey is null || previousSurvey.Status != SurveyStatuses.Rejected)
                return new ValidationError("The previous survey must be a rejected survey.");
        }

        var surveyId = Guid.NewGuid();
        var surveyAssigneds = surveyRequest.SurveyAssigneds
            .Select(x => SurveyAssigned.Create(surveyId, x.EmployeeId, x.IsLead, request.userId))
            .ToList();

        var assignmentNo = await surveyRepository.GenerateAssignmentNoAsync(surveyRequest.SurveyType, surveyRequest.AssignmentDate.Year);

        var survey = Survey.Create(
            surveyId,
            surveyRequest.SurveyType,
            assignmentNo,
            surveyRequest.AssignmentDate,
            surveyRequest.ProposedArea,
            surveyRequest.AdministrativeUnitId,
            surveyRequest.TentativeSubmissionDate,
            surveyRequest.SurveyFormAttached,
            null,                               // branch is only used by Group Survey
            surveyRequest.PreviousSurveyId,
            surveyAssigneds,
            request.userId
        );
        await surveyRepository.AddSurvey(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Survey created successfully.");
    }
}
#endregion
