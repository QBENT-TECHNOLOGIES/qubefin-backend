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
    Task<IReadOnlyList<ApprovalWorkflow>> SearchAsync(string? category, Guid? organizationUnitTypeId, Guid? salaryGradeId);
    Task<IReadOnlyList<ApprovalWorkflow>> GetSiblingsAsync(string category, Guid? organizationUnitTypeId, Guid? leaveTypeId, int minimumDays, int maximumDays);
    Task DeleteAsync(Guid id);
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
            existingStep.OrganizationUnitTypeId = step.OrganizationUnitTypeId;
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
            .Include(m => m.LeaveType)
            .Include(m => m.OrganizationUnitType)
            .Include(m => m.SalaryGrade)
            .Include(m => m.Post)
            .Include(x => x.TblApprovalWorkflowSteps)
            .ThenInclude(x => x.ReceiverPost)
            .Include(x => x.TblApprovalWorkflowSteps)
            .ThenInclude(x => x.OrganizationUnitType)
            .Include(u => u.CreatedByNavigation)
            .Include(u => u.LastModifiedByNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity?.ToDomain();
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetAllAsync()
    {
        var entities = await context.TblApprovalWorkflows
            .Include(m => m.LeaveType)
            .Include(m => m.OrganizationUnitType)
            .Include(m => m.SalaryGrade)
            .Include(m => m.Post)
            .Include(x => x.TblApprovalWorkflowSteps)
            .ThenInclude(x => x.ReceiverPost)
            .AsNoTracking()
            .OrderBy(x => x.Category)
            .ThenBy(x => x.MinimumDays)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public async Task<IEnumerable<ApprovalWorkflow>> GetByCategoryAsync(string category)
    {
        var entities = await context.TblApprovalWorkflows
            .Include(x => x.LeaveType)
            .Include(x => x.OrganizationUnitType)
            .Include(x => x.SalaryGrade)
            .Include(x => x.Post)
            .Include(x => x.TblApprovalWorkflowSteps)
            .ThenInclude(x => x.ReceiverPost)
            .AsNoTracking()
            .Where(x => x.Category == category)
            .OrderBy(x => x.MinimumDays)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public async Task<IReadOnlyList<ApprovalWorkflow>> SearchAsync(string? category, Guid? organizationUnitTypeId, Guid? salaryGradeId)
    {
        var query = context.TblApprovalWorkflows
            .Include(x => x.LeaveType)
            .Include(x => x.OrganizationUnitType)
            .Include(x => x.SalaryGrade)
            .Include(x => x.Post)
            .Include(x => x.CreatedByNavigation)
            .Include(x => x.LastModifiedByNavigation)
            .Include(x => x.TblApprovalWorkflowSteps)
                .ThenInclude(x => x.ReceiverPost)
            .Include(x => x.TblApprovalWorkflowSteps)
                .ThenInclude(x => x.OrganizationUnitType)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => x.Category == category);
        }

        if (organizationUnitTypeId.HasValue && organizationUnitTypeId != Guid.Empty)
        {
            query = query.Where(x => x.OrganizationUnitTypeId == organizationUnitTypeId.Value);
        }

        if (salaryGradeId.HasValue && salaryGradeId != Guid.Empty)
        {
            query = query.Where(x => x.SalaryGradeId == salaryGradeId.Value);
        }

        var entities = await query.ToListAsync();
        return entities.Select(x => x.ToDomain()).ToList();
    }
    public async Task<IReadOnlyList<ApprovalWorkflow>> GetSiblingsAsync(string category, Guid? organizationUnitTypeId, Guid? leaveTypeId, int minimumDays, int maximumDays)
    {
        var entities = await context.TblApprovalWorkflows
            .Include(x => x.TblApprovalWorkflowSteps).ThenInclude(x => x.ReceiverPost)
            .Include(x => x.TblApprovalWorkflowSteps).ThenInclude(x => x.OrganizationUnitType)
            .Include(x => x.SalaryGrade)
            .Where(x => x.Category == category
                && x.OrganizationUnitTypeId == organizationUnitTypeId
                && x.LeaveTypeId == leaveTypeId
                && x.MinimumDays == minimumDays
                && x.MaximumDays == maximumDays)
            .ToListAsync();

        return entities.Select(x => x.ToDomain()).ToList();
    }
    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.TblApprovalWorkflows.FindAsync(id);
        if (entity is not null)
        {
            context.TblApprovalWorkflows.Remove(entity);
        }
    }
}
