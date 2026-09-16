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
    Task<int> CountCreatedInYearAsync(Guid companyId, int year, CancellationToken cancellationToken = default);
}

public class CandidateRepository(QubeFinDataContext context) : ICandidateRepository
{
    public async Task<Candidate?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblInterviewCandidates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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

    public Task<int> CountCreatedInYearAsync(Guid companyId, int year, CancellationToken cancellationToken = default)
    {
        return context.TblInterviewCandidates.AsNoTracking().CountAsync(c => c.CompanyId == companyId && c.CreatedOn.Year == year, cancellationToken);
    }
}
