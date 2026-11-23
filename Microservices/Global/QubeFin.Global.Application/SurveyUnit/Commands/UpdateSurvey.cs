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
public record UpdateSurveyCommand(SurveyRequest SurveyRequest, Guid userId) : IRequest<Result<string>>;
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

        // A Survey Committee member cannot also be a surveyor.
        var assignedEmployeeIds = surveyRequest.SurveyAssigneds.Select(x => x.EmployeeId).ToList();
        if (await surveyCommitteeRepository.AnyActiveMemberAsync(assignedEmployeeIds))
            return new ValidationError("A Survey Committee member cannot be assigned as a surveyor.");

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
            surveyAssigneds,
            request.userId);

        await surveyRepository.UpdateSurvey(survey);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Survey updated successfully.");
    }
}
#endregion
