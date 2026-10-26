using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QubeFin.Persistence;

namespace QubeFin.Report.Application.Reports.Queries;


#region --- QUERY ---
public record GetCompanyByCandidateIdQuery(Guid candidateId) : IRequest<Result<GetCompanyByCandidateIdResponse>>;
#endregion
public record GetCompanyByCandidateIdResponse(Guid companyId);
#region --- VALIDATOR ---
public class GetCompanyByCandidateIdQueryValidator : AbstractValidator<GetCompanyByCandidateIdQuery>
{
    public GetCompanyByCandidateIdQueryValidator()
    {
        RuleFor(v => v.candidateId).NotNull().WithMessage("Candidate is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetCompanyByCandidateIdQueryHandler(QubeFinDataContext context, IMemoryCache cache) : IRequestHandler<GetCompanyByCandidateIdQuery, Result<GetCompanyByCandidateIdResponse>>
{
    public async Task<Result<GetCompanyByCandidateIdResponse>> Handle(GetCompanyByCandidateIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var getCompanyByCandidateIdQuery = await context.TblInterviewCandidates.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.candidateId, cancellationToken: cancellationToken);

            if (getCompanyByCandidateIdQuery == null)
                return Result.Fail<GetCompanyByCandidateIdResponse>("Company not found.");

            return Result.Ok(new GetCompanyByCandidateIdResponse(getCompanyByCandidateIdQuery.CompanyId));
        }
        catch
        {
            return Result.Fail<GetCompanyByCandidateIdResponse>("Something went wrong. Please try again later");
        }
    }
}
#endregion