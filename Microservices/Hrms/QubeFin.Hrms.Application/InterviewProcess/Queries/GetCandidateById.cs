using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidateByIdQuery(Guid CandidateId, Guid employeeId) : IRequest<Result<GetInterviewCandidateDetail>>;

internal sealed class GetCandidateByIdQueryHandler(QubeFinDataContext context, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetCandidateByIdQuery, Result<GetInterviewCandidateDetail>>
{
    public async Task<Result<GetInterviewCandidateDetail>> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var CandidateInterviewInfo = await context.Set<GetInterviewCandidateDetail>().FromSqlRaw("EXEC [Hrms].[USP_GetInterviewCandidateById] @CandidateId, @EmployeeId",
             new SqlParameter("@CandidateId", request.CandidateId),
             new SqlParameter("@EmployeeId", request.employeeId)
            ).AsNoTracking().ToListAsync(cancellationToken);

            if (CandidateInterviewInfo == null || !CandidateInterviewInfo.Any())
                return Result.Fail("Something went wrong. Please try again later.");

            var result = CandidateInterviewInfo.First();
            return Result.Ok(result);
        }
        catch(Exception ex)
        {
            return Result.Fail("Something went wrong while fetching candidate details.");
        }
    }
}
