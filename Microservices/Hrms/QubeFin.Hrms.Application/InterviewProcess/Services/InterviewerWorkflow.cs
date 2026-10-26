using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

public static class InterviewerWorkflow
{
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
