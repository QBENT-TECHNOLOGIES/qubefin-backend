using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            var candidateInterviewInfo = await context.Set<GetInterviewCandidateDetail>()
                                        .FromSqlRaw(
                                            "EXEC [Hrms].[USP_GetInterviewCandidateById] @CandidateId, @EmployeeId",
                                            new SqlParameter("@CandidateId", request.CandidateId),
                                            new SqlParameter("@EmployeeId", request.employeeId)
                                        ).AsNoTracking().ToListAsync(cancellationToken);

            var result = candidateInterviewInfo.FirstOrDefault();
            if (result is null)
                return Result.Fail("Something went wrong. Please try again later.");

            result.CvFileUrl = await UrlOrNullAsync(result.CvFile, cancellationToken);
            result.JobApplicationFileUrl = await UrlOrNullAsync(result.JobApplicationFile, cancellationToken);
            result.WrittenInterviewFIleUrl = await UrlOrNullAsync(result.WrittenInterviewFIle, cancellationToken);
            result.SignedJoiningLetterFileUrl = await UrlOrNullAsync(result.SignedJoiningLetterFile, cancellationToken);

            // The SP masks a missing ReportingTime as 00:00, so the offer letter's joining details are checked on the
            // raw columns - the same rule as Candidate.MissingJoiningDetails.
            result.IsJoiningDetailsComplete = await context.TblInterviewCandidates
                .AsNoTracking()
                .Where(c => c.Id == request.CandidateId)
                .Select(c => c.PostedOrganizationUnitId != null && c.DateOfJoining != null && c.ReportingTime != null &&
                             c.MonthlyCostCompany != null && c.MonthlyCostCompany > 0)
                .FirstOrDefaultAsync(cancellationToken);

            return Result.Ok(result);
        }
        catch (Exception)
        {
            return Result.Fail("Something went wrong while fetching candidate details.");
        }
    }

    private async Task<string?> UrlOrNullAsync(string? key, CancellationToken cancellationToken) =>
        string.IsNullOrEmpty(key) ? null : await fileStorageRepository.GetFileUrlAsync(key, cancellationToken);
}
