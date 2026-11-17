using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using QubeFin.Collection.Persistence.Repositories;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Collection;

namespace QubeFin.Collection.Application.Collections.Commands;

#region --- REQUEST ---
// The collection sheet of one BC group for one collection date, as entered on the BC group collection page.
// DesignationId is the credit officer's designation and BcGroupId the group being saved.
public record SaveBcCollectionRequest(
    Guid DesignationId,
    Guid BcGroupId,
    DateOnly CollectionDate,
    IReadOnlyList<LoanCollectionRequest> Loans);

public record LoanCollectionRequest(
    Guid LoanId,
    decimal CollectionAmount,
    bool IsUpiCollection,
    string? UtrNo,
    string? Remarks);
#endregion

#region --- COMMAND ---
// EmployeeId is the logged-in user, used to keep the save inside their organization units. UserId is written on
// every collection row.
public record SaveBcCollectionCommand(
    Guid EmployeeId,
    Guid UserId,
    SaveBcCollectionRequest Request) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class SaveBcCollectionCommandValidator : AbstractValidator<SaveBcCollectionCommand>
{
    public const int MaxLoansPerSave = 200;

    public SaveBcCollectionCommandValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.Request).NotNull().WithMessage("Collection details are required.");

        When(v => v.Request is not null, () =>
        {
            RuleFor(v => v.Request.DesignationId).NotEmpty().WithMessage("Credit officer is required.");
            RuleFor(v => v.Request.BcGroupId).NotEmpty().WithMessage("BC Group is required.");

            RuleFor(v => v.Request.CollectionDate)
                .NotEqual(default(DateOnly)).WithMessage("Collection date is required.")
                .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today)).WithMessage("Collection date cannot be a future date.");

            RuleFor(v => v.Request.Loans)
                .NotEmpty().WithMessage("Enter collection for at least one loan.")
                .Must(loans => loans.Count <= MaxLoansPerSave).WithMessage($"A save can carry at most {MaxLoansPerSave} loans.")
                .Must(loans => loans.Select(l => l.LoanId).Distinct().Count() == loans.Count).WithMessage("A loan is entered more than once.");

            RuleForEach(v => v.Request.Loans).ChildRules(loan =>
            {
                loan.RuleFor(l => l.LoanId).NotEmpty().WithMessage("Loan Id is required.");
                loan.RuleFor(l => l.CollectionAmount)
                    .GreaterThanOrEqualTo(0).WithMessage("Collection amount cannot be negative.")
                    .PrecisionScale(18, 2, true).WithMessage("Collection amount can have at most 2 decimal places.");
                loan.RuleFor(l => l.CollectionAmount)
                    .GreaterThan(0).When(l => l.IsUpiCollection).WithMessage("A UPI collection must be more than 0.");
                loan.RuleFor(l => l.UtrNo)
                    .NotEmpty().When(l => l.IsUpiCollection).WithMessage("UTR No is required for a UPI collection.");
                loan.RuleFor(l => l.UtrNo)
                    .MaximumLength(50).WithMessage("UTR No cannot be longer than 50 characters.");
                loan.RuleFor(l => l.Remarks)
                    .MaximumLength(500).WithMessage("Remarks cannot be longer than 500 characters.");
            });
        });
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SaveBcCollectionCommandHandler(
    ICollectionRepository collectionRepository,
    ILogger<SaveBcCollectionCommandHandler> logger)
    : IRequestHandler<SaveBcCollectionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveBcCollectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await SaveAsync(request, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Saving BC collection failed for group {BcGroupId} on {CollectionDate}.",
                request.Request.BcGroupId, request.Request.CollectionDate);

            return Result.Fail($"Unable to save the collection. {ex.Message}");
        }
    }

    private async Task<Result<string>> SaveAsync(SaveBcCollectionCommand request, CancellationToken cancellationToken)
    {
        var collectionDate = request.Request.CollectionDate;

        // The group's loans for the collection date, read through the same search the page uses, so a group or loan
        // outside the login user's organization units is never found and cannot be saved.
        var groupLoans = await collectionRepository.GetGroupLoansAsync(
            request.EmployeeId,
            request.Request.DesignationId,
            request.Request.BcGroupId,
            collectionDate,
            new BcGroupLoanSearchParam { PageIndex = 0, PageSize = SaveBcCollectionCommandValidator.MaxLoansPerSave },
            cancellationToken);

        if (groupLoans.Count == 0)
        {
            return Result.Fail(new RecordNotFoundError("BC group not found for this credit officer."));
        }

        var loansById = groupLoans.ToDictionary(l => l.Id);
        var errors = new List<string>();

        foreach (var entry in request.Request.Loans)
        {
            if (!loansById.TryGetValue(entry.LoanId, out var loan))
            {
                errors.Add("A loan does not belong to this BC group.");
                continue;
            }

            if (loan.IsClosed)
            {
                errors.Add($"Loan {loan.LoanNo} is closed.");
            }
            //else if (!loan.IsDemandDate)
            //{
            //    errors.Add($"Loan {loan.LoanNo} has no EMI due on {collectionDate:dd/MM/yyyy}.");
            //}
            //else if (loan.IsCollected && (loan.CollectedAmount ?? 0) > 0)
            //{
            //    errors.Add($"Loan {loan.LoanNo} is already collected for {collectionDate:dd/MM/yyyy}.");
            //}
        }

        if (errors.Count > 0)
        {
            return Result.Fail(new ValidationError(string.Join(" ", errors.Distinct())));
        }

        var entries = request.Request.Loans
            .Select(entry => new LoanCollectionEntry
            {
                LoanId = entry.LoanId,
                CollectionDate = collectionDate.ToDateTime(TimeOnly.MinValue),
                CollectionAmount = entry.CollectionAmount,
                IsUpiCollection = entry.IsUpiCollection,
                UtrNo = entry.IsUpiCollection && !string.IsNullOrWhiteSpace(entry.UtrNo) ? entry.UtrNo.Trim() : null,
                Remarks = string.IsNullOrWhiteSpace(entry.Remarks) ? null : entry.Remarks.Trim(),
                UserId = request.UserId
            })
            .ToList();

        await collectionRepository.SaveLoanCollectionsAsync(entries, cancellationToken);

        var groupName = groupLoans[0].GroupName;
        return Result.Ok($"Collection saved for {entries.Count} loan(s) of {groupName}.");
    }
}
#endregion
