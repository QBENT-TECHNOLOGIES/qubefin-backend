using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidateVerificationQuery(Guid CandidateId) : IRequest<Result<CandidateVerificationDto>>;


internal sealed class GetCandidateVerificationQueryHandler(ICandidateRepository candidateRepository) : IRequestHandler<GetCandidateVerificationQuery, Result<CandidateVerificationDto>>
{
    public async Task<Result<CandidateVerificationDto>> Handle(GetCandidateVerificationQuery request, CancellationToken cancellationToken)
    {
        var result = await candidateRepository.GetVerificationAsync(request.CandidateId, cancellationToken);

        if (result is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        return Result.Ok(result);
    }
}
