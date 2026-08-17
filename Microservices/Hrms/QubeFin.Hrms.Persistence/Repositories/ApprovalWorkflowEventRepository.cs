using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IApprovalWorkflowEventRepository
{
    Task AddAsync(ApprovalWorkflowEvent approvalWorkflowEvent);
    Task UpdateAsync(ApprovalWorkflowEvent approvalWorkflowEvent);
    Task<ApprovalWorkflowEvent?> GetByIdAsync(Guid id);
    Task<IEnumerable<ApprovalWorkflowEvent>> GetAllAsync();
    Task<IEnumerable<ApprovalWorkflowEvent>> GetByCategoryAsync(string category);
    Task<List<TblPost>> GetAllPost();
}

public class ApprovalWorkflowEventRepository(QubeFinDataContext context) : IApprovalWorkflowEventRepository
{
    public async Task AddAsync(ApprovalWorkflowEvent approvalWorkflowEvent)
    {
        await context.TblApprovalWorkflowEvents.AddAsync(approvalWorkflowEvent.ToEntity());
    }

    public Task UpdateAsync(ApprovalWorkflowEvent approvalWorkflowEvent)
    {
        context.TblApprovalWorkflowEvents.Update(approvalWorkflowEvent.ToEntity());
        return Task.CompletedTask;
    }

    public async Task<ApprovalWorkflowEvent?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblApprovalWorkflowEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity?.ToDomain();
    }

    public async Task<IEnumerable<ApprovalWorkflowEvent>> GetAllAsync()
    {
        var entities = await context.TblApprovalWorkflowEvents
            .AsNoTracking()
            .OrderBy(x => x.Category)
            .ThenBy(x => x.SequenceNo)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public async Task<IEnumerable<ApprovalWorkflowEvent>> GetByCategoryAsync(string category)
    {
        var entities = await context.TblApprovalWorkflowEvents
            .AsNoTracking()
            .Where(x => x.Category == category)
            .OrderBy(x => x.SequenceNo)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public async Task<List<TblPost>> GetAllPost()
    {
        return await context.TblPosts.Where(m => m.IsActive).AsNoTracking().ToListAsync();
    }
}
