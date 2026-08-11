using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IApprovalWorkflowRepository
{
    Task AddAsync(ApprovalWorkflow approvalWorkflow);
    Task UpdateAsync(ApprovalWorkflow approvalWorkflow);
    Task<ApprovalWorkflow?> GetByIdAsync(Guid id);
    Task<IEnumerable<ApprovalWorkflow>> GetAllAsync();
    Task<IEnumerable<ApprovalWorkflow>> GetByCategoryAsync(string category);
}

public class ApprovalWorkflowRepository(QubeFinDataContext context) : IApprovalWorkflowRepository
{
    public async Task AddAsync(ApprovalWorkflow approvalWorkflow)
    {
        await context.TblApprovalWorkflows.AddAsync(approvalWorkflow.ToEntity());
    }

    public async Task UpdateAsync(ApprovalWorkflow approvalWorkflow)
    {
        var entity = await context.TblApprovalWorkflows
            .Include(x => x.TblApprovalWorkflowSteps)
            .FirstAsync(x => x.Id == approvalWorkflow.Id);

        entity.Category = approvalWorkflow.Category;
        entity.LeaveTypeId = approvalWorkflow.LeaveTypeId;
        entity.OrganizationUnitTypeId = approvalWorkflow.OrganizationUnitTypeId;
        entity.SalaryGradeId = approvalWorkflow.SalaryGradeId;
        entity.PostId = approvalWorkflow.PostId;
        entity.MinimumDays = approvalWorkflow.MinimumDays;
        entity.MaximumDays = approvalWorkflow.MaximumDays;
        entity.LastModifiedOn = approvalWorkflow.LastModifiedOn;
        entity.LastModifiedBy = approvalWorkflow.LastModifiedBy;

        var existingSteps = entity.TblApprovalWorkflowSteps.ToDictionary(x => x.Id);
        var requestedStepIds = approvalWorkflow.Steps.Select(x => x.Id).ToHashSet();

        context.TblApprovalWorkflowSteps.RemoveRange(
            entity.TblApprovalWorkflowSteps.Where(x => !requestedStepIds.Contains(x.Id)));

        foreach (var step in approvalWorkflow.Steps)
        {
            if (!existingSteps.TryGetValue(step.Id, out var existingStep))
            {
                await context.TblApprovalWorkflowSteps.AddAsync(step.ToEntity());
                continue;
            }

            existingStep.ReceiverPostId = step.ReceiverPostId;
            existingStep.IsRecommendEvent = step.IsRecommendEvent;
            existingStep.IsApprovalEvent = step.IsApprovalEvent;
            existingStep.EventStatus = step.EventStatus;
            existingStep.EventButtonText = step.EventButtonText;
            existingStep.SequenceNo = step.SequenceNo;
        }
    }

    public async Task<ApprovalWorkflow?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblApprovalWorkflows
            .Include(x => x.TblApprovalWorkflowSteps)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity?.ToDomain();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetAllAsync()
    {
        var entities = await context.TblApprovalWorkflows
            .Include(x => x.TblApprovalWorkflowSteps)
            .AsNoTracking()
            .OrderBy(x => x.Category)
            .ThenBy(x => x.MinimumDays)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetByCategoryAsync(string category)
    {
        var entities = await context.TblApprovalWorkflows
            .Include(x => x.TblApprovalWorkflowSteps)
            .AsNoTracking()
            .Where(x => x.Category == category)
            .OrderBy(x => x.MinimumDays)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }
}
