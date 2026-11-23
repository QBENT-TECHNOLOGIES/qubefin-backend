using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyEvaluations
{
    public static class SurveyEvaluationRules
    {
        // The committee can evaluate from the moment the Lead Surveyor submits, until the Lead decides.
        public static bool IsOpenForEvaluation(string status)
        {
            return status == SurveyStatuses.PendingEvaluation
                || status == SurveyStatuses.EvaluationInProgress
                || status == SurveyStatuses.PendingLeadEvaluation;
        }
    }
}
