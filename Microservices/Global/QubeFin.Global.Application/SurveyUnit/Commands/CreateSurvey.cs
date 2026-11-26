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
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateSurveyCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var surveyRequest = request.SurveyRequest;

        var assignedEmployeeIds = surveyRequest.SurveyAssigneds.Select(x => x.EmployeeId).ToList();
        Guid? branchId = null;
        Guid? companyId = null;

        if (surveyRequest.SurveyType == SurveyTypes.Group)
        {
            // Only whoever is in charge of the branch can assign a Group Survey, and only for that branch.
            var branch = await surveyRepository.GetHeadBranchAsync(request.employeeId);
            if (branch is null)
                return new ForbiddenError("Only the Head of a branch (the person in charge, or the Branch Manager when nobody is in charge) can create a Group Survey.");
            if (surveyRequest.BranchOrganizationUnitId != branch.Id)
                return new ForbiddenError("A Group Survey can only be created for the branch you are in charge of.");
            branchId = branch.Id;

            // A Group Survey takes the company of its branch.
            companyId = await surveyRepository.GetBranchCompanyIdAsync(branch.Id);
            if (companyId is null)
                return new ValidationError("The branch has no company, so a Group Survey cannot be created for it.");

            var problem = await GroupSurveyRules.CheckCreditOfficersAsync(
                surveyRepository, surveyCommitteeRepository, branch.Id, request.employeeId, assignedEmployeeIds);
            if (problem is not null)
                return new ValidationError(problem);
        }
        else
        {
            // Only the Committee Lead can assign a Branch Survey.
            var isCommitteeLead = await surveyCommitteeRepository.IsActiveLeadAsync(request.employeeId);
            if (!isCommitteeLead)
                return new ForbiddenError("Only the Committee Lead can create a Branch Survey.");

            // A Survey Committee member can also be a surveyor of a Branch Survey (and then evaluates it like the others).

            // A Branch Survey is created for the company chosen in the dropdown.
            companyId = surveyRequest.CompanyId;
            if (!companyId.HasValue || !await surveyRepository.CompanyExistsAsync(companyId.Value))
                return new ValidationError("Select the company of the survey.");
        }

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
            branchId,                           // branch is only used by Group Survey
            surveyRequest.PreviousSurveyId,
            companyId,
            surveyAssigneds,
            request.userId
        );
        await surveyRepository.AddSurvey(survey);

        // Tell the Credit Officers about their new Group Survey.
        if (surveyRequest.SurveyType == SurveyTypes.Group)
        {
            await notificationRepository.AddForEmployeesAsync(
                assignedEmployeeIds,
                title: "Group Survey Assigned",
                message: $"{assignmentNo} ({surveyRequest.ProposedArea}) has been assigned to you.",
                icon: "clipboard-check",
                referenceId: surveyId,
                referenceType: "SURVEY",
                actionUrl: "/secure/global/surveys",
                createdBy: request.userId);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Survey created successfully.");
    }
}
#endregion
