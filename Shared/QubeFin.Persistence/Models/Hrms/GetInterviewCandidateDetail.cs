namespace QubeFin.Persistence.Models.Hrms
{
    public class GetInterviewCandidateDetail
    {
        public Guid? Id { get; set; }
        public string? ReferenceNo { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? CandidateFullName { get; set; }
        public string? Gender { get; set; }
        public string? FatherName { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? HouseNo { get; set; }
        public string? RoadName { get; set; }
        public string? LandMark { get; set; }
        public Guid? AdministrativeUnitId { get; set; }
        public Guid? PoliceStationId { get; set; }
        public Guid? PostOfficeId { get; set; }
        public string? PinCode { get; set; }
        public string? Location { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public DateOnly? InterviewDate { get; set; }
        public TimeOnly? InterviewTime { get; set; }
        public string? WrittenInterviewFIle { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? InterviewPost { get; set; }
        public string? InterviewPostName { get; set; }
        public Guid? VenueOrganizationUnitId { get; set; }
        public string? Venue { get; set; }
        public string? InterviewMode { get; set; }
        public string? ReferedBy { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public int? NoticePeriodInDays { get; set; }
        public DateOnly? EarliestJoiningDate { get; set; }
        public bool? IsWillingRelocate { get; set; }
        public string? PreferredLocation { get; set; }
        public string? OverallPerformance { get; set; }
        public string? SuitableRoleDepartment { get; set; }
        public Guid? RecommendedGradeId { get; set; }
        public string? RecommendedGradeName { get; set; }
        public bool? IsTrainingRequired { get; set; }
        public string? RecommendationStatus { get; set; }
        public int? TotalRatingPoint { get; set; }
        public string? RatingStatus { get; set; }
        public string? RecruitmentSource { get; set; }
        public string? VacancyReference { get; set; }
        public string? AadharNumber { get; set; }
        public bool? IsAadharValidated { get; set; }
        public string? VoterNumber { get; set; }
        public bool? IsVoterValited { get; set; }
        public string? Pan { get; set; }
        public bool? IsPanValidated { get; set; }
        public bool? IsMobileValidated { get; set; }
        public string? Uan { get; set; }
        public bool? IsUanVerified { get; set; }
        public bool? IsCreditBureauChecked { get; set; }
        public string? CreditBureauReportLink { get; set; }
        public Guid? PostedOrganizationUnitId { get; set; }
        public string? PostedOrganizationUnitName { get; set; }
        public DateOnly? DateOfJoining { get; set; }
        public TimeOnly? ReportingTime { get; set; }
        public decimal? MonthlyCostCompany { get; set; }
        public bool? IsInterviewLetterRecieved { get; set; }
        public bool? IsOfferLetterReceived { get; set; }
        public bool? IsAppointmentLetterReceived { get; set; }
        public bool? IsWelcomeLetterRecieved { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? IsHR { get; set; }
        public bool? IsInterviewLetterReceived { get; set; }
        public bool? IsInterviewerAcknowledged { get; set; }
        public bool? ShowCreatePanelButton { get; set; }
        public bool? IsPanelCreated { get; set; }
        public int? PanelMemberCount { get; set; }
        public bool? IsCurrentEmployeePanelMember { get; set; }
        public bool? CanAcknowledgePanel { get; set; }
        public bool? IsCurrentEmployeeAttended { get; set; }
        public bool? IsCurrentEmployeeAssessmentSubmitted { get; set; }
        public bool? IsAllPanelAcknowledged { get; set; }
        public bool? IsAllPanelAssessmentSubmitted { get; set; }
        public bool? IsShowHrAssessmentButton { get; set; }
        public bool? IsHrAssessmentCompleted { get; set; }
        public bool? IsCandidateQualified { get; set; }
        public bool? IsShowCandidateVerificationButton { get; set; }
        public bool? IsCandidateVerificationCompleted { get; set; }
        public bool? CanGenerateOfferLetter { get; set; }
        public bool? IsOfferLetterGenerated { get; set; }
        public string? CurrentWorkflowStage { get; set; }
        public bool? IsInterviewerAcknowledgedOld { get; set; }
        public bool? OldIsShowHrAssessmentButton { get; set; }
        public bool? ShowViewPanelButton { get; set; }
        public bool? IsAssessmentDate { get; set; }

        /// <summary>HR has started the HR Assessment and saved it as a draft, but has not submitted it yet.
        /// IsShowHrAssessmentButton stays true in this state so the form can be reopened.</summary>
        public bool? IsHrAssessmentDraftSaved { get; set; }

        /// <summary>HR's own row in Hrms.Tbl_InterviewPanel has been finalised.</summary>
        public bool? IsHrAssessmentSubmitted { get; set; }

        /// <summary>Total of the ten averaged category ratings stored on HR's assessment row.</summary>
        public int? HrAssessmentTotalRatingPoint { get; set; }

        /// <summary>How many interviewers (excluding HR's assessment row) have acknowledged.</summary>
        public int? InterviewerAcknowledgedCount { get; set; }

        /// <summary>How many interviewers (excluding HR's assessment row) have submitted their assessment.</summary>
        public int? InterviewerSubmittedCount { get; set; }
    }
}
