using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>
/// Hrms.Tbl_InterviewPanel holds two different kinds of row: the interviewers HR scheduled onto the panel,
/// and the single row HR writes when they fill in the HR Assessment (the averaged ratings have to be stored
/// somewhere, and the panel table is the only table with a column per rating category).
///
/// Nothing on the table itself distinguishes the two, and we are not allowed to add a column, so both this
/// layer and USP_GetInterviewCandidateById identify the HR row the same way: by the PostId behind the panel
/// member's current designation. Keep the GUID below in sync with @HRPostId in that stored procedure.
/// </summary>
public static class InterviewPanelRoles
{
    /// <summary>PostId of the HR post - mirrors @HRPostId in Hrms.USP_GetInterviewCandidateById.</summary>
    public static readonly Guid HrPostId = new("416B4D7C-DB96-426B-BA19-0FB46D1940FF");

    /// <summary>True when this panel row belongs to HR rather than to an interviewer, i.e. it is the row the
    /// HR Assessment writes to.</summary>
    public static bool IsHrRow(this InterviewPanel panel) => panel.DesignationPostId == HrPostId;

    /// <summary>The interviewer rows for a candidate: every panel row except HR's own assessment row. These
    /// are the rows whose ratings get averaged, and the rows the "everyone has submitted" gate looks at.</summary>
    public static IEnumerable<InterviewPanel> Interviewers(this IEnumerable<InterviewPanel> panel) =>
        panel.Where(p => !p.IsHrRow());

    /// <summary>True when HR's own panel row shows they were genuinely scheduled onto the panel as an
    /// interviewer, rather than only holding the administrative row the HR Assessment save/submit flow
    /// creates for a pure (non-interviewer) HR. This is the one reliable existing signal without adding a
    /// column: Acknowledge() is only ever called by the ordinary panel-invitation flow
    /// (AcknowledgePanelInvitationCommand) - the HR Assessment flow never touches IsAcknowledged on the row
    /// it creates or updates. Used to decide whether HR's own row should count toward the interviewer
    /// aggregates and averages, and whether the HR Assessment save/submit flow may safely write into it.</summary>
    public static bool IsGenuineInterviewer(this InterviewPanel? hrOwnRow) => hrOwnRow is { IsAcknowledged: true };
}
