using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.GroupSurveys.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.GroupSurveys.Commands;

#region --- COMMAND ---
public record SaveGroupSurveyDraftCommand(Guid SurveyId, GroupSurveyForm Form, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveGroupSurveyDraftCommandValidator : AbstractValidator<SaveGroupSurveyDraftCommand>
{
    public SaveGroupSurveyDraftCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Form).NotNull().WithMessage("Survey form is required.");
        RuleFor(v => v.Form).SetValidator(new GroupSurveyFormValidator()).When(v => v.Form != null);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SaveGroupSurveyDraftCommandHandler(
    ISurveyRepository surveyRepository,
    IGroupSurveyRepository groupSurveyRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveGroupSurveyDraftCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveGroupSurveyDraftCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("This survey does not have a group survey form.");

        // The form can be changed only until the Lead Credit Officer submits it.
        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("The survey form cannot be changed after it is submitted.");

        // Only the Lead Credit Officer writes the form. The other Credit Officer and everyone else only read it.
        var isLeadCreditOfficer = survey.SurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        if (!isLeadCreditOfficer)
            return new ForbiddenError("Only the Lead Credit Officer can fill in the survey form.");

        var groupSurvey = await groupSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (groupSurvey is null)
        {
            groupSurvey = new TblGroupSurvey
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                CreatedBy = request.UserId,
                CreatedOn = DateTime.Now
            };
            groupSurvey.ApplyForm(request.Form);
            await groupSurveyRepository.AddAsync(groupSurvey);
        }
        else
        {
            groupSurvey.ApplyForm(request.Form);
            groupSurvey.LastModifiedBy = request.UserId;
            groupSurvey.LastModifiedOn = DateTime.Now;
        }

        // The place names come from the survey area, not from the screen.
        var places = await groupSurveyRepository.GetPlaceNamesAsync(survey.AdministrativeUnitId);
        groupSurvey.VillageLocality = places.VillageLocality;
        groupSurvey.GramPanchayatMunicipality = places.GramPanchayatMunicipality;
        groupSurvey.Block = places.Block;

        // The first save moves the survey from Assigned to Draft.
        await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Draft, request.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Survey form saved as draft.");
    }
}
#endregion
