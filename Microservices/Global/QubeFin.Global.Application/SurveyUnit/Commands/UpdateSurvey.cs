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
public record UpdateSurveyCommand(SurveyRequest SurveyRequest, Guid userId, Guid employeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateSurveyCommandValidator : AbstractValidator<UpdateSurveyCommand>
{
    public UpdateSurveyCommandValidator()
    {
        RuleFor(v => v.SurveyRequest).NotNull().WithMessage("Survey request is required.");
        RuleFor(v => v.SurveyRequest.Id).NotEmpty().WithMessage("Survey Id is required.").When(v => v.SurveyRequest != null);
        RuleFor(v => v.SurveyRequest).SetValidator(new SurveyRequestValidator()).When(v => v.SurveyRequest != null);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateSurveyCommandHandler(
    ISurveyRepository surveyRepository,
    ISurveyCommitteeRepository surveyCommitteeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateSurveyCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        var surveyRequest = request.SurveyRequest;

        var survey = await surveyRepository.GetByIdAsync(surveyRequest.Id);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        // Only the user who created the survey can change it.
        if (survey.CreatedBy != request.userId)
            return new ForbiddenError("Only the user who created the survey can edit it.");

        // The assignment can be changed only until the Lead Surveyor submits the survey.
        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("The survey cannot be edited after it is submitted.");

        var assignedEmployeeIds = surveyRequest.SurveyAssigneds.Select(x => x.EmployeeId).ToList();
        var companyId = survey.CompanyId;
        if (survey.SurveyType == SurveyTypes.Group)
        {
            // Only the person in charge of the survey's branch can change a Group Survey.
            var branch = await surveyRepository.GetHeadBranchAsync(request.employeeId);
            if (branch is null || branch.Id != survey.BranchOrganizationUnitId)
                return new ForbiddenError("Only the Head of the branch can edit this Group Survey.");

            // Credit Officers who are already on the survey are not checked again.
            var currentEmployeeIds = survey.SurveyAssigneds.Select(x => x.EmployeeId).ToHashSet();
            var newEmployeeIds = assignedEmployeeIds.Where(id => !currentEmployeeIds.Contains(id)).ToList();
            var problem = await GroupSurveyRules.CheckCreditOfficersAsync(
                surveyRepository, surveyCommitteeRepository, branch.Id, request.employeeId, newEmployeeIds);
            if (problem is not null)
                return new ValidationError(problem);

            // The company of a Group Survey is the company of its branch (a survey from before companies gets it now).
            companyId ??= await surveyRepository.GetBranchCompanyIdAsync(branch.Id);
        }
        else
        {
            // A Survey Committee member can also be a surveyor of a Branch Survey (and then evaluates it like the others).

            // The Committee Lead can change the company of a Branch Survey while it can still be edited.
            companyId = surveyRequest.CompanyId;
            if (!companyId.HasValue || !await surveyRepository.CompanyExistsAsync(companyId.Value))
                return new ValidationError("Select the company of the survey.");
        }

        // Employees already assigned keep their original "assigned by" and date. Only new ones get the current user.
        var surveyAssigneds = new List<SurveyAssigned>();
        foreach (var assigned in surveyRequest.SurveyAssigneds)
        {
            var current = survey.SurveyAssigneds.FirstOrDefault(x => x.Id == assigned.Id);
            if (current is null)
                surveyAssigneds.Add(SurveyAssigned.Create(survey.Id, assigned.EmployeeId, assigned.IsLead, request.userId));
            else
                surveyAssigneds.Add(new SurveyAssigned(current.Id, survey.Id, assigned.EmployeeId, assigned.IsLead, current.AssignedBy));
        }

        survey.Update(
            surveyRequest.AssignmentDate,
            surveyRequest.ProposedArea,
            surveyRequest.AdministrativeUnitId,
            surveyRequest.TentativeSubmissionDate,
            surveyRequest.SurveyFormAttached,
            survey.BranchOrganizationUnitId,    // branch is not changed here
            companyId,
            surveyAssigneds,
            request.userId);

        await surveyRepository.UpdateSurvey(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Survey updated successfully.");
    }
}
#endregion
