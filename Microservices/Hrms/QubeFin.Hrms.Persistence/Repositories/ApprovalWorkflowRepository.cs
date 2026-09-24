using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
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
    Task<bool> HasConflictingWorkflowAsync(IReadOnlyCollection<Guid> excludedWorkflowIds, string category, Guid? organizationUnitTypeId, Guid? leaveTypeId, int minimumDays, int maximumDays, IReadOnlyCollection<Guid> salaryGradeIds);
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

        // Workflow fields
        entity.Category = approvalWorkflow.Category;
        entity.LeaveTypeId = approvalWorkflow.LeaveTypeId;
        entity.OrganizationUnitTypeId = approvalWorkflow.OrganizationUnitTypeId;
        entity.SalaryGradeId = approvalWorkflow.SalaryGradeId;
        entity.PostId = approvalWorkflow.PostId;
        entity.MinimumDays = approvalWorkflow.MinimumDays;
        entity.MaximumDays = approvalWorkflow.MaximumDays;
        entity.LastModifiedOn = approvalWorkflow.LastModifiedOn;
        entity.LastModifiedBy = approvalWorkflow.LastModifiedBy;

        // Only live steps take part in the sync. Steps that were soft deleted
        // earlier stay untouched because the approval history still points at them.
        var activeSteps = entity.TblApprovalWorkflowSteps
            .Where(x => !x.IsDelete)
            .ToList();

        var stepsById = activeSteps
            .ToDictionary(x => x.Id);

        // The step IDs in the request belong to the workflow the user was editing.
        // Sibling workflows (one row per salary grade) carry their own step rows,
        // so they are matched on SequenceNo instead.
        var stepsBySequenceNo = activeSteps
            .GroupBy(x => x.SequenceNo)
            .ToDictionary(x => x.Key, x => x.First());

        var matchedStepIds = new HashSet<Guid>();

        foreach (var step in approvalWorkflow.Steps)
        {
            var existingStep = FindStep(
                step,
                stepsById,
                stepsBySequenceNo,
                matchedStepIds);

            if (existingStep is not null)
            {
                matchedStepIds.Add(existingStep.Id);

                existingStep.ReceiverPostId = step.ReceiverPostId;
                existingStep.OrganizationUnitTypeId = step.OrganizationUnitTypeId;
                existingStep.IsRecommendEvent = step.IsRecommendEvent;
                existingStep.IsApprovalEvent = step.IsApprovalEvent;
                existingStep.EventStatus = step.EventStatus;
                existingStep.EventButtonText = step.EventButtonText;
                existingStep.SequenceNo = step.SequenceNo;

                continue;
            }

            // New step. A fresh ID is always generated because the ID carried by
            // the request may already belong to a step row of another workflow.
            var newStep = step.ToEntity();

            newStep.Id = Guid.NewGuid();
            newStep.ApprovalWorkflowId = entity.Id;
            newStep.IsDelete = false;

            context.TblApprovalWorkflowSteps.Add(newStep);
        }

        var removedSteps = activeSteps
            .Where(x => !matchedStepIds.Contains(x.Id))
            .ToList();

        await RemoveStepsAsync(removedSteps);
    }

    public async Task<ApprovalWorkflow?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblApprovalWorkflows
            .Include(m => m.LeaveType)
            .Include(m => m.OrganizationUnitType)
            .Include(m => m.SalaryGrade)
            .Include(m => m.Post)
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete))
            .ThenInclude(x => x.ReceiverPost)
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete))
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
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete))
            .ThenInclude(x => x.ReceiverPost)
            .AsNoTracking()
            .Where(x => x.TblApprovalWorkflowSteps.Any(s => !s.IsDelete))
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
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete))
            .ThenInclude(x => x.ReceiverPost)
            .AsNoTracking()
            .Where(x => x.Category == category
                && x.TblApprovalWorkflowSteps.Any(s => !s.IsDelete))
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
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete))
                .ThenInclude(x => x.ReceiverPost)
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete))
                .ThenInclude(x => x.OrganizationUnitType)
            .AsNoTracking()
            .Where(x => x.TblApprovalWorkflowSteps.Any(s => !s.IsDelete))
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
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete)).ThenInclude(x => x.ReceiverPost)
            .Include(x => x.TblApprovalWorkflowSteps.Where(s => !s.IsDelete)).ThenInclude(x => x.OrganizationUnitType)
            .Include(x => x.SalaryGrade)
            .Where(x => x.Category == category
                && x.OrganizationUnitTypeId == organizationUnitTypeId
                && x.LeaveTypeId == leaveTypeId
                && x.MinimumDays == minimumDays
                && x.MaximumDays == maximumDays
                && x.TblApprovalWorkflowSteps.Any(s => !s.IsDelete))
            .ToListAsync();

        return entities.Select(x => x.ToDomain()).ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.TblApprovalWorkflows
            .Include(x => x.TblApprovalWorkflowSteps)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            return;
        }

        var activeSteps = entity.TblApprovalWorkflowSteps
            .Where(x => !x.IsDelete)
            .ToList();

        await RemoveStepsAsync(activeSteps);

        // The workflow row can only leave the table when none of its steps had
        // to be kept for the approval history.
        var hasRemainingSteps = entity.TblApprovalWorkflowSteps
            .Any(x => context.Entry(x).State != EntityState.Deleted);

        if (!hasRemainingSteps)
        {
            context.TblApprovalWorkflows.Remove(entity);
        }
    }

    public async Task<bool> HasConflictingWorkflowAsync(IReadOnlyCollection<Guid> excludedWorkflowIds, string category, Guid? organizationUnitTypeId, Guid? leaveTypeId, int minimumDays, int maximumDays, IReadOnlyCollection<Guid> salaryGradeIds)
    {
        return await context.TblApprovalWorkflows.AsNoTracking()
            .AnyAsync(x =>
                !excludedWorkflowIds.Contains(x.Id) &&
                x.Category == category &&
                x.OrganizationUnitTypeId == organizationUnitTypeId &&
                x.LeaveTypeId == leaveTypeId &&
                x.MinimumDays == minimumDays &&
                x.MaximumDays == maximumDays &&
                x.SalaryGradeId.HasValue &&
                salaryGradeIds.Contains(x.SalaryGradeId.Value) &&
                x.TblApprovalWorkflowSteps.Any(s => !s.IsDelete));
    }

    // =============================================================
    // Matches a requested step against a step row of this workflow:
    // first on ID, then on SequenceNo for sibling workflows whose
    // step IDs the request does not know about.
    // =============================================================
    private static TblApprovalWorkflowStep? FindStep(
        ApprovalWorkflowStep step,
        IReadOnlyDictionary<Guid, TblApprovalWorkflowStep> stepsById,
        IReadOnlyDictionary<int, TblApprovalWorkflowStep> stepsBySequenceNo,
        HashSet<Guid> matchedStepIds)
    {
        if (step.Id != Guid.Empty &&
            stepsById.TryGetValue(step.Id, out var stepById) &&
            !matchedStepIds.Contains(stepById.Id))
        {
            return stepById;
        }

        if (stepsBySequenceNo.TryGetValue(step.SequenceNo, out var stepBySequenceNo) &&
            !matchedStepIds.Contains(stepBySequenceNo.Id))
        {
            return stepBySequenceNo;
        }

        return null;
    }

    // =============================================================
    // A step already referenced by Tbl_ApprovalRequestEvent cannot be
    // deleted, so it is soft deleted instead. Steps that were never
    // used are removed for real.
    // =============================================================
    private async Task RemoveStepsAsync(IReadOnlyCollection<TblApprovalWorkflowStep> steps)
    {
        if (steps.Count == 0)
        {
            return;
        }

        var stepIds = steps
            .Select(x => x.Id)
            .ToList();

        var references = await context.TblApprovalRequestEvents
            .AsNoTracking()
            .Where(x => stepIds.Contains(x.ApprovalWorkflowStepId)
                || (x.NextApprovalWorkflowStepId != null
                    && stepIds.Contains(x.NextApprovalWorkflowStepId.Value)))
            .Select(x => new
            {
                x.ApprovalWorkflowStepId,
                x.NextApprovalWorkflowStepId
            })
            .ToListAsync();

        var usedStepIds = new HashSet<Guid>(
            references.Select(x => x.ApprovalWorkflowStepId));

        foreach (var reference in references)
        {
            if (reference.NextApprovalWorkflowStepId.HasValue)
            {
                usedStepIds.Add(reference.NextApprovalWorkflowStepId.Value);
            }
        }

        foreach (var step in steps)
        {
            if (usedStepIds.Contains(step.Id))
            {
                step.IsDelete = true;
                continue;
            }

            context.TblApprovalWorkflowSteps.Remove(step);
        }
    }
}
