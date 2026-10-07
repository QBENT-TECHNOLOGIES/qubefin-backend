using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>When an interviewer may record attendance and fill in their assessment. Mirrors the CanStartAssessment /
/// CanContinueAssessment flags of Hrms.USP_GetInterviewerCandidateList - keep the two in step.</summary>
public static class InterviewerWorkflow
{
    /// <summary>Null when the interviewer may work on this panel row today, otherwise why not. The assessment is
    /// open on the interview date only, to an interviewer who acknowledged, until HR completes the HR Assessment.</summary>
    public static async Task<string?> GetAssessmentBlockerAsync(this QubeFinDataContext context, InterviewPanel panel, CancellationToken cancellationToken)
    {
        var candidate = await context.TblInterviewCandidates
            .AsNoTracking()
            .Where(c => c.Id == panel.CandidateId)
            .Select(c => new { c.InterviewDate, c.IsHrAssessmentCompleted })
            .FirstOrDefaultAsync(cancellationToken);

        if (candidate is null)
        {
            return "Candidate not found.";
        }

        if (candidate.IsHrAssessmentCompleted || candidate.InterviewDate is null || candidate.InterviewDate < CandidateWorkflow.Today)
        {
            return "This interview is closed.";
        }

        if (candidate.InterviewDate > CandidateWorkflow.Today)
        {
            return "The assessment opens on the interview date.";
        }

        if (!panel.IsAcknowledged)
        {
            return "Acknowledge the interview before starting the assessment.";
        }

        return null;
    }
}
