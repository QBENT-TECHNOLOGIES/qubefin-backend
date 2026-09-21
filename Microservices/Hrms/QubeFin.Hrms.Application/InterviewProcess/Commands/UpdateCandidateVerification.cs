using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>
/// Manually records the outcome of the candidate's background/identity verification checks. There is no
/// external verification API wired up yet (Aadhar/Voter/PAN/UAN/credit-bureau are all just booleans in the
/// database), so this is a straightforward "HR/Admin ticks the boxes they've personally checked" update -
/// all six flags are sent together in one call, unlike the one-flag-per-call letter-status endpoint.
/// </summary>
public record UpdateCandidateVerificationCommand(
    Guid CandidateId,
    bool IsAadharValidated,
    bool IsVoterValited,
    bool IsPanValidated,
    bool IsMobileValidated,
    bool IsUanVerified,
    bool IsCreditBureauChecked,
    string? CreditBureauReportLink,
    Guid ModifiedBy) : IRequest<Result<CandidateVerificationDto>>;

public class UpdateCandidateVerificationCommandValidator : AbstractValidator<UpdateCandidateVerificationCommand>
{
    public UpdateCandidateVerificationCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
    }
}

internal sealed class UpdateCandidateVerificationCommandHandler(
    ICandidateRepository candidateRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCandidateVerificationCommand, Result<CandidateVerificationDto>>
{
    public async Task<Result<CandidateVerificationDto>> Handle(UpdateCandidateVerificationCommand request, CancellationToken cancellationToken)
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

        candidate.UpdateVerification(
            request.IsAadharValidated,
            request.IsVoterValited,
            request.IsPanValidated,
            request.IsMobileValidated,
            request.IsUanVerified,
            request.IsCreditBureauChecked,
            request.CreditBureauReportLink,
            request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var overallStatus =
            candidate.IsAadharValidated &&
            candidate.IsVoterValited &&
            candidate.IsPanValidated &&
            candidate.IsMobileValidated &&
            candidate.IsCreditBureauChecked
                ? "Verified"
                : "Pending";

        var dto = new CandidateVerificationDto
        {
            CandidateId = candidate.Id,
            AadharNumber = candidate.AadharNumber,
            IsAadharValidated = candidate.IsAadharValidated,
            VoterNumber = candidate.VoterNumber,
            IsVoterValited = candidate.IsVoterValited,
            Pan = candidate.Pan,
            IsPanValidated = candidate.IsPanValidated,
            MobileNo = candidate.MobileNo,
            IsMobileValidated = candidate.IsMobileValidated,
            Uan = candidate.Uan,
            IsUanVerified = candidate.IsUanVerified,
            IsCreditBureauChecked = candidate.IsCreditBureauChecked,
            CreditBureauReportLink = candidate.CreditBureauReportLink,
            OverallStatus = overallStatus
        };

        return Result.Ok(dto);
    }
}
