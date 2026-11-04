using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Jobs.Models;

namespace QubeFin.Tracking.Application.Jobs;

// D4: who may transfer, cancel or update a job
internal static class JobPermissionRules
{
    public static JobPermissions For(TrackingViewer viewer, JobDetailRow job)
    {
        // The assignee gets nothing on the web: no transfer, no passing down, no adding tasks
        if (viewer.EmployeeId.HasValue && job.Summary.AssigneeId == viewer.EmployeeId) return new JobPermissions(false, false, false);

        var isCreatorOrAbove = job.CreatedBy == viewer.UserId || IsAbove(viewer, job.Creator);
        var isBranchManagerOfAbm = viewer.Role == TrackingRole.BranchManager
            && job.Assignee.PostName == TrackingRole.AssistantBranchManager
            && job.Assignee.UnitId == viewer.UnitId;
        var canUpdate = isCreatorOrAbove || IsAbove(viewer, job.Assignee) || isBranchManagerOfAbm;

        return new JobPermissions(isCreatorOrAbove, isCreatorOrAbove, canUpdate);
    }

    // Higher role, with the other person's unit inside the viewer's scope
    private static bool IsAbove(TrackingViewer viewer, TrackingPosition other)
    {
        if (other.IsAdmin) return false;
        if (viewer.Role == TrackingRole.Admin) return true;
        if (other.PostName is null || other.UnitId is null) return false;
        return TrackingRole.Rank(viewer.Role) > TrackingRole.Rank(other.PostName)
            && viewer.UnitIdsInScope.Contains(other.UnitId.Value);
    }
}
