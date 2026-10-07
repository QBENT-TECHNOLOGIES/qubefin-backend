using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IInterviewPanelRepository
{
    Task<InterviewPanel?> GetByIdAsync(Guid id);
    Task<List<InterviewPanel>> GetByCandidateIdAsync(Guid candidateId, bool includeEmployee = true);
    Task<InterviewPanel?> GetByCandidateAndEmployeeAsync(Guid candidateId, Guid employeeId, bool includeEmployee = true);
    Task<int> AcknowledgeAsync(Guid employeeId, IReadOnlyCollection<Guid> candidateIds, DateOnly today, Guid acknowledgedBy, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<InterviewPanel> panelists, CancellationToken cancellationToken = default);
    Task UpdateAsync(InterviewPanel panel);
    Task DeleteAsync(Guid id);
}

public class InterviewPanelRepository(QubeFinDataContext context) : IInterviewPanelRepository
{
    public async Task<InterviewPanel?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblInterviewPanels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity?.ToDomain();
    }
    public async Task<List<InterviewPanel>> GetByCandidateIdAsync(Guid candidateId, bool includeEmployee = true)
    {
        var entities = await WithEmployee(includeEmployee)
            .Where(x => x.CandidateId == candidateId)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<InterviewPanel?> GetByCandidateAndEmployeeAsync(Guid candidateId, Guid employeeId, bool includeEmployee = true)
    {
        var entity = await WithEmployee(includeEmployee)
            .FirstOrDefaultAsync(x => x.CandidateId == candidateId && x.EmployeeId == employeeId);

        return entity?.ToDomain();
    }
    public Task<int> AcknowledgeAsync(Guid employeeId, IReadOnlyCollection<Guid> candidateIds, DateOnly today, Guid acknowledgedBy, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return context.TblInterviewPanels
            .Where(p => p.EmployeeId == employeeId
                     && candidateIds.Contains(p.CandidateId)
                     && !p.IsAcknowledged
                     && p.Candidate.InterviewDate != null
                     && p.Candidate.InterviewDate >= today
                     && (p.Candidate.RecommendationStatus == null || p.Candidate.RecommendationStatus != Candidate.RejectedRecommendationStatus)
                     && !p.Candidate.IsHrAssessmentCompleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.IsAcknowledged, true)
                .SetProperty(p => p.AcknowledgedDate, now)
                .SetProperty(p => p.ModifiedBy, acknowledgedBy)
                .SetProperty(p => p.ModifiedOn, now), cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<InterviewPanel> panelists, CancellationToken cancellationToken = default)
    {
        await context.TblInterviewPanels.AddRangeAsync(panelists.Select(p => p.ToEntity()), cancellationToken);
    }

    public Task UpdateAsync(InterviewPanel panel)
    {
        context.TblInterviewPanels.Update(panel.ToEntity());
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.TblInterviewPanels.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return;

        context.TblInterviewPanels.Remove(entity);
    }

    private IQueryable<TblInterviewPanel> WithEmployee(bool includeEmployee)
    {
        IQueryable<TblInterviewPanel> query = context.TblInterviewPanels.AsNoTracking();

        return includeEmployee
            ? query.Include(m => m.Employee).ThenInclude(m => m.TblEmployeeDesignations).ThenInclude(m => m.Designation)
            : query;
    }
}
