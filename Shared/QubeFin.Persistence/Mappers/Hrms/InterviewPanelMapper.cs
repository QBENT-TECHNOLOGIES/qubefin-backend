using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class InterviewPanelMapper
{
    public static InterviewPanel ToDomain(this TblInterviewPanel entity)
    {
        return new InterviewPanel(
            entity.Id,
            entity.CandidateId,
            entity.EmployeeId,
            entity.ScheduledDate,
            entity.ScheduledTime,
            entity.IsAcknowledged,
            entity.AcknowledgedDate,
            entity.IsAttened,
            entity.AppearanceAttitudeRating,
            entity.AppearanceAttitudeRemarks,
            entity.PersonalityRating,
            entity.PersonalityRemarks,
            entity.CommunicationRating,
            entity.CommunicationRemarks,
            entity.EducationRating,
            entity.EducationRemarks,
            entity.WorkExperienceRating,
            entity.WorkExperienceRemarks,
            entity.TechnicalCompetenceRating,
            entity.TechnicalCompetenceRemarks,
            entity.FlexibilityRating,
            entity.FlexibilityRemarks,
            entity.AmbitionRating,
            entity.AmbitionRemarks,
            entity.PotentialRating,
            entity.PotentialRemarks,
            entity.OthersRating,
            entity.OthersRemarks,
            entity.AnyOtherJobsSuitedRemarks,
            entity.IsRecommendedForPosition,
            entity.PositiveRemarks,
            entity.NegativeRemarks,
            entity.IsSubmitted,
            entity.SubmissionDate,
            entity.ModifiedBy,
            entity.ModifiedOn);
    }

    public static TblInterviewPanel ToEntity(this InterviewPanel panel)
    {
        return new TblInterviewPanel
        {
            Id = panel.Id,
            CandidateId = panel.CandidateId,
            EmployeeId = panel.EmployeeId,
            ScheduledDate = panel.ScheduledDate,
            ScheduledTime = panel.ScheduledTime,
            IsAcknowledged = panel.IsAcknowledged,
            AcknowledgedDate = panel.AcknowledgedDate,
            IsAttened = panel.IsAttened,
            AppearanceAttitudeRating = panel.AppearanceAttitudeRating ?? 0,
            AppearanceAttitudeRemarks = panel.AppearanceAttitudeRemarks,
            PersonalityRating = panel.PersonalityRating ?? 0,
            PersonalityRemarks = panel.PersonalityRemarks,
            CommunicationRating = panel.CommunicationRating ?? 0,
            CommunicationRemarks = panel.CommunicationRemarks,
            EducationRating = panel.EducationRating ?? 0,
            EducationRemarks = panel.EducationRemarks,
            WorkExperienceRating = panel.WorkExperienceRating ?? 0,
            WorkExperienceRemarks = panel.WorkExperienceRemarks,
            TechnicalCompetenceRating = panel.TechnicalCompetenceRating ?? 0,
            TechnicalCompetenceRemarks = panel.TechnicalCompetenceRemarks,
            FlexibilityRating = panel.FlexibilityRating ?? 0,
            FlexibilityRemarks = panel.FlexibilityRemarks,
            AmbitionRating = panel.AmbitionRating ?? 0,
            AmbitionRemarks = panel.AmbitionRemarks,
            PotentialRating = panel.PotentialRating ?? 0,
            PotentialRemarks = panel.PotentialRemarks,
            OthersRating = panel.OthersRating ?? 0,
            OthersRemarks = panel.OthersRemarks,
            AnyOtherJobsSuitedRemarks = panel.AnyOtherJobsSuitedRemarks,
            IsRecommendedForPosition = panel.IsRecommendedForPosition ?? false,
            PositiveRemarks = panel.PositiveRemarks,
            NegativeRemarks = panel.NegativeRemarks,
            IsSubmitted = panel.IsSubmitted,
            SubmissionDate = panel.SubmissionDate,
            ModifiedBy = panel.ModifiedBy,
            ModifiedOn = panel.ModifiedOn
        };
    }
}