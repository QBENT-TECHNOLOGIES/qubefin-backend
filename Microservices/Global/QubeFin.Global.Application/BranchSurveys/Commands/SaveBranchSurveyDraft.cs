using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Application.BranchSurveys.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.BranchSurveys.Commands;

#region --- COMMAND ---
public record SaveBranchSurveyDraftCommand(Guid SurveyId, BranchSurveyForm Form, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveBranchSurveyDraftCommandValidator : AbstractValidator<SaveBranchSurveyDraftCommand>
{
    public SaveBranchSurveyDraftCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Form).NotNull().WithMessage("Survey form is required.");
        RuleFor(v => v.Form).SetValidator(new BranchSurveyFormValidator()).When(v => v.Form != null);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SaveBranchSurveyDraftCommandHandler(
    ISurveyRepository surveyRepository,
    IBranchSurveyRepository branchSurveyRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveBranchSurveyDraftCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveBranchSurveyDraftCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.SurveyType != SurveyTypes.Branch)
            return new ValidationError("This survey does not have a branch survey form.");

        // The form can be changed only until the Lead Surveyor submits it.
        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("The survey form cannot be changed after it is submitted.");

        // Only the Lead Surveyor writes the form. Other surveyors, committee members and the Committee Lead only read it.
        var isLeadSurveyor = survey.SurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        if (!isLeadSurveyor)
            return new ForbiddenError("Only the Lead Surveyor can fill in the survey form.");

        var branchSurvey = await branchSurveyRepository.GetBySurveyIdAsync(request.SurveyId);
        if (branchSurvey is null)
        {
            branchSurvey = new TblBranchSurvey
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                CreatedBy = request.UserId,
                CreatedOn = DateTime.Now
            };
            branchSurvey.ApplyForm(request.Form);
            await branchSurveyRepository.AddAsync(branchSurvey);
        }
        else
        {
            branchSurvey.ApplyForm(request.Form);
            branchSurvey.LastModifiedBy = request.UserId;
            branchSurvey.LastModifiedOn = DateTime.Now;
        }

        branchSurveyRepository.ReplaceCompetitors(branchSurvey, request.Form.ToCompetitorEntities(branchSurvey.Id, request.UserId));

        // The first save moves the survey from Assigned to Draft.
        await surveyRepository.UpdateStatusAsync(request.SurveyId, SurveyStatuses.Draft, request.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok("Survey form saved as draft.");
    }
}
#endregion
