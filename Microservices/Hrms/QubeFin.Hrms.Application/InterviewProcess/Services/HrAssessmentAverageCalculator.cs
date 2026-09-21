using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;

/// <summary>The ten scored categories, averaged across every panelist who has submitted their assessment
/// for a candidate. Used to pre-fill (and, on save, persist) the read-only rating section of the HR
/// Assessment form - HR sees the average, not a per-panelist number.</summary>
public record HrAssessmentAverages(
    int? AppearanceAttitudeRating,
    int? PersonalityRating,
    int? CommunicationRating,
    int? EducationRating,
    int? WorkExperienceRating,
    int? TechnicalCompetenceRating,
    int? FlexibilityRating,
    int? AmbitionRating,
    int? PotentialRating,
    int? OthersRating)
{
    /// <summary>Sum of the ten averaged categories - mirrors InterviewPanel.TotalRatingPoint for a single panelist.</summary>
    public int? Total
    {
        get
        {
            var values = new[]
            {
                AppearanceAttitudeRating, PersonalityRating, CommunicationRating, EducationRating,
                WorkExperienceRating, TechnicalCompetenceRating, FlexibilityRating, AmbitionRating,
                PotentialRating, OthersRating
            };

            return values.Any(v => v.HasValue)
                ? values.Where(v => v.HasValue).Sum(v => v!.Value)
                : null;
        }
    }
}

public static class HrAssessmentAverageCalculator
{
    /// <summary>Averages each of the ten rating categories across the given (already-submitted) panelists,
    /// rounding to the nearest whole number since the underlying columns are integers. A category with no
    /// ratings at all comes back null rather than 0.</summary>
    public static HrAssessmentAverages Compute(IEnumerable<InterviewPanel> submittedPanelists)
    {
        var panelists = submittedPanelists.ToList();

        return new HrAssessmentAverages(
            Average(panelists.Select(p => p.AppearanceAttitudeRating)),
            Average(panelists.Select(p => p.PersonalityRating)),
            Average(panelists.Select(p => p.CommunicationRating)),
            Average(panelists.Select(p => p.EducationRating)),
            Average(panelists.Select(p => p.WorkExperienceRating)),
            Average(panelists.Select(p => p.TechnicalCompetenceRating)),
            Average(panelists.Select(p => p.FlexibilityRating)),
            Average(panelists.Select(p => p.AmbitionRating)),
            Average(panelists.Select(p => p.PotentialRating)),
            Average(panelists.Select(p => p.OthersRating)));
    }

    /// <summary>Buckets a total score (max 50 = 10 categories x 5) into a rating status label.
    /// Adjust these thresholds if your organization uses different bands.</summary>
    public static string RatingStatusFor(int? totalRatingPoint)
    {
        if (!totalRatingPoint.HasValue)
        {
            return "Not started";
        }

        return totalRatingPoint.Value switch
        {
            >= 40 => "Excellent",
            >= 30 => "Good",
            >= 20 => "Average",
            _ => "Below Average"
        };
    }

    private static int? Average(IEnumerable<int?> ratings)
    {
        var values = ratings.Where(r => r.HasValue).Select(r => r!.Value).ToList();
        return values.Count == 0
            ? null
            : (int)Math.Round(values.Average(), MidpointRounding.AwayFromZero);
    }
}
