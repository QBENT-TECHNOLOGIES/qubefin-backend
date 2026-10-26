using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Services;
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
