using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Mobilization.Commands;

#region --- COMMAND ---
public record ChangeBorrowerDecisionCommand(Guid SurveyId, Guid BorrowerId, BorrowerDecisionRequest Decision, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class ChangeBorrowerDecisionCommandValidator : AbstractValidator<ChangeBorrowerDecisionCommand>
{
    public ChangeBorrowerDecisionCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.BorrowerId).NotEmpty().WithMessage("Borrower Id is required.");
        RuleFor(v => v.Decision).NotNull();
        RuleFor(v => v.Decision.Decision).Must(d => d == BorrowerDecisions.Add || d == BorrowerDecisions.Block).WithMessage("Decision must be Add or Block.");
        RuleFor(v => v.Decision.Reason).NotEmpty().WithMessage("Give the reason for your decision.").MaximumLength(500).WithMessage("Reason can be 500 characters at most.");
    }
}
#endregion

#region --- HANDLER ---
// R-14: the Head of the branch sees flagged borrowers highlighted and can change Add to Block or Block to Add. No approval step.
internal sealed class ChangeBorrowerDecisionCommandHandler(
    QubeFinDataContext context,
    ISurveyRepository surveyRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeBorrowerDecisionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ChangeBorrowerDecisionCommand request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group || survey.Status != SurveyStatuses.Approved || survey.BranchOrganizationUnitId is null)
            return new ValidationError("This survey is not open for Community Mobilization.");
        if (!await surveyRepository.IsHeadOfBranchAsync(request.EmployeeId, survey.BranchOrganizationUnitId.Value))
            return new ForbiddenError("Only the Head of the branch can change the decision.");

        var borrower = await context.TblGroupBorrowers.FirstOrDefaultAsync(b => b.Id == request.BorrowerId && b.SurveyId == request.SurveyId, cancellationToken);
        if (borrower is null)
            return new RecordNotFoundError("Borrower not found.");
        if (!borrower.IsFlagged)
            return new ValidationError("Only a flagged borrower has a decision to change.");
        if (borrower.GroupId != null || (borrower.Status != BorrowerStatuses.Pending && borrower.Status != BorrowerStatuses.Blocked))
            return new ValidationError("This borrower can no longer be changed.");

        borrower.Status = request.Decision.Decision == BorrowerDecisions.Block ? BorrowerStatuses.Blocked : BorrowerStatuses.Pending;
        borrower.DecisionReason = request.Decision.Reason!.Trim();
        borrower.LastModifiedBy = request.UserId;
        borrower.LastModifiedOn = DateTime.Now;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(borrower.Status == BorrowerStatuses.Blocked ? "Borrower blocked." : "Borrower added.");
    }
}
#endregion
