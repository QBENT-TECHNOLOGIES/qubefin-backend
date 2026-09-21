using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

/// <summary>Whether the candidate's signed joining letter has been uploaded yet - drives the frontend's
/// "show Welcome Letter once Joining Letter is uploaded" gate. Reads straight off Tbl_InterviewCandidate via
/// EF rather than through USP_GetInterviewCandidateById, since that stored procedure isn't touched by this
/// feature.</summary>
public record GetCandidateJoiningLetterStatusQuery(Guid CandidateId) : IRequest<Result<CandidateJoiningLetterStatusDto>>;

internal sealed class GetCandidateJoiningLetterStatusQueryHandler(
    ICandidateRepository candidateRepository,
    IFileStorageRepository fileStorageRepository) : IRequestHandler<GetCandidateJoiningLetterStatusQuery, Result<CandidateJoiningLetterStatusDto>>
{
    public async Task<Result<CandidateJoiningLetterStatusDto>> Handle(GetCandidateJoiningLetterStatusQuery request, CancellationToken cancellationToken)
    {
        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        if (string.IsNullOrEmpty(candidate.SignedJoiningLetterFile))
        {
            return Result.Ok(new CandidateJoiningLetterStatusDto(false, null));
        }

        var fileUrl = await fileStorageRepository.GetFileUrlAsync(candidate.SignedJoiningLetterFile, cancellationToken);
        return Result.Ok(new CandidateJoiningLetterStatusDto(true, fileUrl));
    }
}
