using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models;

// ================================================================
// A workflow is stored as one row per salary grade. Rows that share
// the same category / organization unit type / leave type / day
// range are siblings, but only the ones walking the SAME approval
// path belong to the same logical workflow.
//
// Example:
//
// Branch Manager:
//     V, VI, VII, VIII, IX
//
// Area Manager:
//     X, XI
//
// Both share the day range, yet they are different workflows.
// ================================================================
internal static class ApprovalWorkflowPath
{
    // Salary Grade is deliberately NOT part of the key.
    public static string GetKey(ApprovalWorkflow workflow)
    {
        return string.Join(
            "|",
            (workflow.Steps ?? new List<ApprovalWorkflowStep>())
                .OrderBy(s => s.SequenceNo)
                .Select(s =>
                    $"{s.OrganizationUnitTypeId}:" +
                    $"{s.ReceiverPostId}:" +
                    $"{s.SequenceNo}"));
    }
}
