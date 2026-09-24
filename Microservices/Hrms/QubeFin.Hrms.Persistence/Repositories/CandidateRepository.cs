using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface ICandidateRepository
{
    Task<Candidate?> GetByIdAsync(Guid id);
    Task AddAsync(Candidate candidate, CancellationToken cancellationToken = default);
    Task UpdateAsync(Candidate candidate);
    Task<int> CountCreatedInYearAsync(CancellationToken cancellationToken = default);
    Task<CandidateVerificationDto?> GetVerificationAsync(Guid candidateId, CancellationToken cancellationToken = default);
    Task<Guid?> GetEmployeeIdAsync(Guid candidateId, CancellationToken cancellationToken = default);
}

public class CandidateRepository(QubeFinDataContext context) : ICandidateRepository
{
    public async Task<Candidate?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblInterviewCandidates.Include(m => m.InterviewPostNavigation).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity?.ToDomain();
    }

    public async Task AddAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        await context.TblInterviewCandidates.AddAsync(candidate.ToEntity(), cancellationToken);
    }

    public Task UpdateAsync(Candidate candidate)
    {
        context.TblInterviewCandidates.Update(candidate.ToEntity());
        return Task.CompletedTask;
    }

    public Task<int> CountCreatedInYearAsync(CancellationToken cancellationToken = default)
    {
        return context.TblInterviewCandidates.AsNoTracking().CountAsync(cancellationToken);
    }

    public Task<Guid?> GetEmployeeIdAsync(Guid candidateId, CancellationToken cancellationToken = default)
    {
        return context.TblEmployees
            .AsNoTracking()
            .Where(x => x.CandidateId == candidateId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CandidateVerificationDto?> GetVerificationAsync(Guid candidateId, CancellationToken cancellationToken = default)
    {
        return await context.TblInterviewCandidates
            .AsNoTracking()
            .Where(x => x.Id == candidateId)
            .Select(x => new CandidateVerificationDto
            {
                CandidateId = x.Id,

                AadharNumber = x.AadharNumber,
                IsAadharValidated = x.IsAadharValidated,

                VoterNumber = x.VoterNumber,
                IsVoterValited = x.IsVoterValited,

                Pan = x.Pan,
                IsPanValidated = x.IsPanValidated,

                MobileNo = x.MobileNo,
                IsMobileValidated = x.IsMobileValidated,

                Uan = x.Uan,
                IsUanVerified = x.IsUanVerified,

                IsCreditBureauChecked = x.IsCreditBureauChecked,
                CreditBureauReportLink = x.CreditBureauReportLink,

                OverallStatus =
                    x.IsAadharValidated &&
                    x.IsVoterValited &&
                    x.IsPanValidated &&
                    x.IsMobileValidated &&
                    x.IsCreditBureauChecked
                        ? "Verified"
                        : "Pending"
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
