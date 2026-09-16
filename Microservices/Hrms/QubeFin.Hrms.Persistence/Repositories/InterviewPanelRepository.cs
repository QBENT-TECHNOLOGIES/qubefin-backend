using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IInterviewPanelRepository
{
    Task<InterviewPanel?> GetByIdAsync(Guid id);
    Task<List<InterviewPanel>> GetByCandidateIdAsync(Guid candidateId);
    Task<InterviewPanel?> GetByCandidateAndEmployeeAsync(Guid candidateId, Guid employeeId);
    Task AddRangeAsync(IEnumerable<InterviewPanel> panelists, CancellationToken cancellationToken = default);
    Task UpdateAsync(InterviewPanel panel);
}

public class InterviewPanelRepository(QubeFinDataContext context) : IInterviewPanelRepository
{
    public async Task<InterviewPanel?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblInterviewPanels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity?.ToDomain();
    }

    public async Task<List<InterviewPanel>> GetByCandidateIdAsync(Guid candidateId)
    {
        var entities = await context.TblInterviewPanels
            .AsNoTracking()
            .Where(x => x.CandidateId == candidateId)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<InterviewPanel?> GetByCandidateAndEmployeeAsync(Guid candidateId, Guid employeeId)
    {
        var entity = await context.TblInterviewPanels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CandidateId == candidateId && x.EmployeeId == employeeId);

        return entity?.ToDomain();
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
}