using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Mobilization.Commands;

#region --- COMMAND ---
// Adds a prospective borrower (BorrowerId empty) or updates one.
public record SaveBorrowerCommand(Guid SurveyId, Guid? BorrowerId, BorrowerRequest Borrower, Guid UserId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveBorrowerCommandValidator : AbstractValidator<SaveBorrowerCommand>
{
    public SaveBorrowerCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.Borrower).NotNull().SetValidator(new BorrowerRequestValidator());
    }
}
#endregion

#region --- HANDLER ---
// The Credit Officers of an Approved Group Survey add and edit the prospective borrowers.
// A person with NPA or a Write-off needs a decision (Add / Block) and a reason; this is checked here, not trusted from the screen.
// Income and pending loan only set the two Yes / No fields (they never block).
internal sealed class SaveBorrowerCommandHandler(
    QubeFinDataContext context,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveBorrowerCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveBorrowerCommand request, CancellationToken cancellationToken)
    {
        var input = request.Borrower;

        var survey = await context.TblSurveys.AsNoTracking()
            .Include(s => s.TblSurveyAssigneds)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");
        if (survey.SurveyType != SurveyTypes.Group)
            return new ValidationError("Community Mobilization is only for Group Surveys.");
        if (survey.Status != SurveyStatuses.Approved)
            return new ValidationError("Borrowers can be added only after the Group Survey is Approved.");
        if (!survey.TblSurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId))
            return new ForbiddenError("Only the Credit Officers of this survey can add borrowers.");

        TblGroupBorrower? borrower = null;
        if (request.BorrowerId.HasValue)
        {
            borrower = await context.TblGroupBorrowers.FirstOrDefaultAsync(b => b.Id == request.BorrowerId && b.SurveyId == request.SurveyId, cancellationToken);
            if (borrower is null)
                return new RecordNotFoundError("Borrower not found.");
            if (borrower.GroupId != null || (borrower.Status != BorrowerStatuses.Pending && borrower.Status != BorrowerStatuses.Blocked))
                return new ValidationError("This borrower can no longer be changed.");
        }

        var breaches = BorrowerEligibility.GetBreaches(input.HasNpa, input.HasWriteOff);
        var reason = (input.DecisionReason ?? string.Empty).Trim();
        if (breaches.Count > 0)
        {
            if (input.Decision is null)
                return new ValidationError("This borrower breaks an eligibility rule. Choose Add or Block.");
            if (reason.Length == 0)
                return new ValidationError("Give the reason for your decision.");
        }

        var now = DateTime.Now;
        if (borrower is null)
        {
            borrower = new TblGroupBorrower
            {
                Id = Guid.NewGuid(),
                SurveyId = request.SurveyId,
                CreatedBy = request.UserId,
                CreatedOn = now
            };
            await context.TblGroupBorrowers.AddAsync(borrower, cancellationToken);
        }
        else
        {
            borrower.LastModifiedBy = request.UserId;
            borrower.LastModifiedOn = now;
        }

        borrower.Name = input.Name.Trim();
        borrower.GuardianName = input.GuardianName.Trim();
        borrower.MobileNo = input.MobileNo.Trim();
        borrower.MaritalStatus = input.MaritalStatus;
        borrower.Age = input.Age;
        borrower.OwnHouse = input.OwnHouse;
        borrower.IncomeActivity = string.IsNullOrWhiteSpace(input.IncomeActivity) ? null : input.IncomeActivity.Trim();
        borrower.Income = input.Income;
        borrower.LoanAmount = input.LoanAmount;
        // The two Yes/No fields of the document, set from the amounts.
        borrower.YearlyIncome = input.Income <= BorrowerEligibility.MaxYearlyIncome;
        borrower.Indebtedness = input.LoanAmount <= BorrowerEligibility.MaxIndebtedness;
        borrower.HasNpa = input.HasNpa;
        borrower.HasWriteOff = input.HasWriteOff;

        borrower.IsFlagged = breaches.Count > 0;
        borrower.FlagReasons = breaches.Count > 0 ? string.Join("; ", breaches) : null;
        borrower.DecisionReason = breaches.Count > 0 ? reason : null;
        borrower.Status = breaches.Count > 0 && input.Decision == BorrowerDecisions.Block
            ? BorrowerStatuses.Blocked
            : BorrowerStatuses.Pending;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(borrower.Status == BorrowerStatuses.Blocked
            ? "Borrower saved as Blocked."
            : "Borrower saved successfully.");
    }
}
#endregion
