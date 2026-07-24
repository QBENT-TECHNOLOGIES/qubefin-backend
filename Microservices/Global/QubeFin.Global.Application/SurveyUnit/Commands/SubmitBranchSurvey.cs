using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.SurveyUnit.Commands;

#region --- COMMAND ---
public record SubmitBranchSurveyCommand(SubmitBranchSurveyRequest Submit, Guid SubmittedBy) : IRequest<Result<SubmitBranchSurveyResponse>>;
#endregion

#region --- VALIDATION ---
public class SubmitBranchSurveyCommandValidator : AbstractValidator<SubmitBranchSurveyCommand>
{
    public SubmitBranchSurveyCommandValidator()
    {
        RuleFor(v => v.Submit).NotNull().WithMessage("Submit request is required.");
        RuleFor(v => v.Submit.SurveyId).NotEmpty().WithMessage("Survey ID is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record SubmitBranchSurveyResponse(bool Created, string Message);
#endregion

#region --- HANDLER ---
internal sealed class SubmitBranchSurveyCommandHandler(ISurveyRepository surveyRepository, IUnitOfWork unitOfWork) : IRequestHandler<SubmitBranchSurveyCommand, Result<SubmitBranchSurveyResponse>>
{
    public async Task<Result<SubmitBranchSurveyResponse>> Handle(SubmitBranchSurveyCommand request, CancellationToken cancellationToken)
    {
        await surveyRepository.SubmitBranchSurvey(request.Submit.Id, request.Submit.IsApproved, request.SubmittedBy);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new SubmitBranchSurveyResponse(true, "Branch survey submitted successfully."));
    }
}
#endregion