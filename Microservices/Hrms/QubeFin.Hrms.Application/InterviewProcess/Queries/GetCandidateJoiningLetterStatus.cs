using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;
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
