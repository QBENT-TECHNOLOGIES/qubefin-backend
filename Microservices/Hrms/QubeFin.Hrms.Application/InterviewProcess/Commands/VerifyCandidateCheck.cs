using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>
/// Verifies one check from the Candidate Verification form ("Verify" button on that row). There is no external
/// verification API yet, so this sets the check's flag on the candidate and saves the value HR entered.
/// </summary>
public record VerifyCandidateCheckCommand(Guid CandidateId, CandidateVerificationCheck Check, string? Value, Guid ModifiedBy) : IRequest<Result<CandidateVerificationDto>>;

public record VerifyCandidateCheckRequest(CandidateVerificationCheck Check, string? Value);

public class VerifyCandidateCheckCommandValidator : AbstractValidator<VerifyCandidateCheckCommand>
{
    public VerifyCandidateCheckCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Check).IsInEnum().WithMessage("Unknown verification check.");
        RuleFor(x => x.Value).MaximumLength(500).WithMessage("Value cannot exceed 500 characters.");

        // Same formats the verification form checks. Blank is allowed - the saved value is kept.
        RuleFor(x => x.Value)
            .Must((command, value) => Formats[command.Check].Regex.IsMatch(value!.Trim()))
            .When(x => !string.IsNullOrWhiteSpace(x.Value) && Formats.ContainsKey(x.Check))
            .WithMessage(x => Formats[x.Check].Message);
    }

    private static readonly Dictionary<CandidateVerificationCheck, (Regex Regex, string Message)> Formats = new()
    {
        [CandidateVerificationCheck.Aadhar] = (new Regex(@"^\d{12}$"), "Aadhaar number must be exactly 12 digits."),
        [CandidateVerificationCheck.Pan] = (new Regex(@"^[A-Z]{5}\d{4}[A-Z]$", RegexOptions.IgnoreCase), "PAN must be in the format ABCDE1234F."),
        [CandidateVerificationCheck.Voter] = (new Regex(@"^[A-Z]{3}\d{7}$", RegexOptions.IgnoreCase), "Voter ID must be 3 letters followed by 7 digits."),
        [CandidateVerificationCheck.Mobile] = (new Regex(@"^[6-9]\d{9}$"), "Enter a valid 10-digit mobile number."),
        [CandidateVerificationCheck.Uan] = (new Regex(@"^\d{12}$"), "UAN must be exactly 12 digits."),
        [CandidateVerificationCheck.CreditBureau] = (new Regex(@"^https?://\S+$", RegexOptions.IgnoreCase), "Enter a valid link starting with http:// or https://."),
    };
}

internal sealed class VerifyCandidateCheckCommandHandler(
    ICandidateRepository candidateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<VerifyCandidateCheckCommand, Result<CandidateVerificationDto>>
{
    public async Task<Result<CandidateVerificationDto>> Handle(VerifyCandidateCheckCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        candidate.VerifyCheck(request.Check, request.Value, request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var verification = await candidateRepository.GetVerificationAsync(request.CandidateId, cancellationToken);
        return verification is null
            ? new RecordNotFoundError("Candidate not found.")
            : Result.Ok(verification);
    }
}
