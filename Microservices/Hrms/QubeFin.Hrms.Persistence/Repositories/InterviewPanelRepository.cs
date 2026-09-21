using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IInterviewPanelRepository
{
    Task<InterviewPanel?> GetByIdAsync(Guid id);
    Task<InterviewPanel?> GetByEmployeeIdAsync(Guid id, Guid employeeId);
    Task<List<InterviewPanel>> GetByCandidateIdAsync(Guid candidateId);
    Task<InterviewPanel?> GetByCandidateAndEmployeeAsync(Guid candidateId, Guid employeeId);
    Task<InterviewPanel?> GetHrAssessmentRowAsync(Guid candidateId);
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
    public async Task<InterviewPanel?> GetByEmployeeIdAsync(Guid id, Guid employeeId)
    {
        // The interviewer workflow (acknowledge) must never land on the HR Assessment row, which can exist
        // for the same employee on the same candidate.
        var entity = await context.TblInterviewPanels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CandidateId == id
                                   && x.EmployeeId == employeeId
                                   && x.AssessmentType == InterviewPanel.InterviewerAssessmentType);
        return entity?.ToDomain();
    }

    public async Task<List<InterviewPanel>> GetByCandidateIdAsync(Guid candidateId)
    {
        var entities = await context.TblInterviewPanels.Include(m => m.Employee).ThenInclude(m => m.TblEmployeeDesignations).ThenInclude(m => m.Designation)
            .AsNoTracking()
            .Where(x => x.CandidateId == candidateId)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    /// <summary>An employee's own INTERVIEWER row for a candidate. Scoped to AssessmentType so the
    /// interviewer save/submit flow can never pick up the HR Assessment row of an HR employee who is also
    /// a panel interviewer.</summary>
    public async Task<InterviewPanel?> GetByCandidateAndEmployeeAsync(Guid candidateId, Guid employeeId)
    {
        var entity = await context.TblInterviewPanels
            .Include(m => m.Employee).ThenInclude(m => m.TblEmployeeDesignations).ThenInclude(m => m.Designation)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CandidateId == candidateId
                                   && x.EmployeeId == employeeId
                                   && x.AssessmentType == InterviewPanel.InterviewerAssessmentType);

        return entity?.ToDomain();
    }

    /// <summary>The candidate's single HR Assessment row, if one has been created yet.</summary>
    public async Task<InterviewPanel?> GetHrAssessmentRowAsync(Guid candidateId)
    {
        var entity = await context.TblInterviewPanels
            .Include(m => m.Employee).ThenInclude(m => m.TblEmployeeDesignations).ThenInclude(m => m.Designation)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CandidateId == candidateId
                                   && x.AssessmentType == InterviewPanel.HrAssessmentType);

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

    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.TblInterviewPanels.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return;

        context.TblInterviewPanels.Remove(entity);
    }
}