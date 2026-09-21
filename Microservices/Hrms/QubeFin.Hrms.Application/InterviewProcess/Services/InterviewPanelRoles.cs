using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>
/// Hrms.Tbl_InterviewPanel holds two different kinds of row: the interviewers HR scheduled onto the panel
/// (AssessmentType = 'INTERVIEWER'), and the single row the HR Assessment writes to
/// (AssessmentType = 'HR', which is where the averaged ratings and HR's own decision remarks live).
///
/// AssessmentType is the ONLY source of truth for that distinction. The employee's HR designation must
/// never be used for it: an HR employee can also be scheduled as a genuine panel interviewer, in which
/// case they hold BOTH an 'INTERVIEWER' row (their own interview) and, once they start the HR Assessment,
/// an 'HR' row (their decision). Keep this in step with USP_GetInterviewCandidateById, which filters on
/// the same column.
/// </summary>
public static class InterviewPanelRoles
{
    /// <summary>True when this row is the HR Assessment row rather than a scheduled interviewer's row.</summary>
    public static bool IsHrAssessmentRow(this InterviewPanel panel) =>
        string.Equals(panel.AssessmentType, InterviewPanel.HrAssessmentType, StringComparison.OrdinalIgnoreCase);

    /// <summary>True when this row is a scheduled panel interviewer's row - including one belonging to an
    /// HR employee who genuinely sits on the panel.</summary>
    public static bool IsInterviewerRow(this InterviewPanel panel) => !panel.IsHrAssessmentRow();

    /// <summary>The interviewer rows for a candidate: every panel row except the HR Assessment row. These
    /// are the rows whose ratings get averaged, and the rows the "everyone has submitted" gate looks at.</summary>
    public static IEnumerable<InterviewPanel> Interviewers(this IEnumerable<InterviewPanel> panel) =>
        panel.Where(p => p.IsInterviewerRow());

    /// <summary>The candidate's HR Assessment row, if HR has started one. At most one exists per candidate.</summary>
    public static InterviewPanel? HrAssessmentRow(this IEnumerable<InterviewPanel> panel) =>
        panel.FirstOrDefault(p => p.IsHrAssessmentRow());

    /// <summary>This employee's own interviewer row on the panel, if they are scheduled as one. Used to tell
    /// "HR is genuinely a panel interviewer" from "HR only holds the HR Assessment row" - never their
    /// designation, and never the HR row's IsSubmitted.</summary>
    public static InterviewPanel? InterviewerRowFor(this IEnumerable<InterviewPanel> panel, Guid employeeId) =>
        panel.FirstOrDefault(p => p.IsInterviewerRow() && p.EmployeeId == employeeId);
}
