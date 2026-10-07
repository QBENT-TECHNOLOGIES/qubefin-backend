namespace QubeFin.Persistence.Models.Hrms
{
    /// <summary>Result of Hrms.USP_GetInterviewCandidateById - the Candidate page's detail for HR / Admin.
    /// Every Show*/Can* flag is decided by the procedure so the UI never derives the workflow order itself.</summary>
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
        public string? Address { get; set; }
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
        public DateOnly? ApplicationDate { get; set; }
        public DateOnly? InterviewDate { get; set; }
        public TimeOnly? InterviewTime { get; set; }

        public string? CvFile { get; set; }
        public string? JobApplicationFile { get; set; }
        public string? WrittenInterviewFIle { get; set; }
        public string? SignedJoiningLetterFile { get; set; }
        public string? CvFileUrl { get; set; }
        public string? JobApplicationFileUrl { get; set; }
        public string? WrittenInterviewFIleUrl { get; set; }
        public string? SignedJoiningLetterFileUrl { get; set; }

        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? InterviewPost { get; set; }
        public string? InterviewPostName { get; set; }
        public Guid? VenueOrganizationUnitId { get; set; }
        public string? Venue { get; set; }
        public string? InterviewMode { get; set; }
        public string? ReferedBy { get; set; }
        public string? RecruitmentSource { get; set; }
        public string? VacancyReference { get; set; }
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
        public bool? IsInterviewLetterReceived { get; set; }
        public bool? IsOfferLetterReceived { get; set; }
        public bool? IsAppointmentLetterReceived { get; set; }
        public bool? IsWelcomeLetterRecieved { get; set; }
        public bool? IsSelectedForOffer { get; set; }
        public bool? IsHrAssessmentCompleted { get; set; }

        // ---- Roles / stopped state ----

        /// <summary>The caller holds the HR post.</summary>
        public bool? IsHR { get; set; }

        /// <summary>The caller created the candidate (and is not HR) - Admin for this candidate only.</summary>
        public bool? IsAdmin { get; set; }

        /// <summary>The caller may act on the candidate right now: HR until the workflow stops, Admin only until
        /// HR saves the HR Assessment draft.</summary>
        public bool? CanAct { get; set; }

        /// <summary>RecommendationStatus 'Rejected'. No further action is allowed.</summary>
        public bool? IsRejected { get; set; }

        /// <summary>HR submitted the HR Assessment with a status outside the qualified list. No further action.</summary>
        public bool? IsNotSelected { get; set; }

        public bool? CanEditDetails { get; set; }

        // ---- Interview ----
        public bool? ShowInterviewLetterActions { get; set; }
        public bool? ShowCreatePanelButton { get; set; }
        public bool? IsPanelCreated { get; set; }
        public int? PanelMemberCount { get; set; }
        public bool? ShowViewPanelButton { get; set; }
        public bool? CanModifyPanel { get; set; }
        public bool? IsAllPanelAcknowledged { get; set; }

        /// <summary>Every panelist is finished - submitted, recorded the candidate absent, or the interview day passed.</summary>
        public bool? IsAllPanelAssessmentSubmitted { get; set; }
        public int? InterviewerAcknowledgedCount { get; set; }
        public int? InterviewerSubmittedCount { get; set; }

        /// <summary>Panelists who have neither submitted nor recorded the candidate absent. HR is warned about them
        /// when opening the HR Assessment; their assessment is left out of the average.</summary>
        public int? PendingPanelAssessmentCount { get; set; }
        public bool? IsWrittenAssessmentUploaded { get; set; }
        public bool? ShowInterviewFormatActions { get; set; }

        // ---- HR Assessment -> selection ----
        public bool? IsShowHrAssessmentButton { get; set; }
        public bool? IsHrAssessmentDraftSaved { get; set; }
        public bool? IsHrAssessmentSubmitted { get; set; }
        public bool? IsCandidateQualified { get; set; }
        public bool? ShowSelectForOfferButton { get; set; }

        // ---- Verification -> post-offer document chain ----
        public bool? IsShowCandidateVerificationButton { get; set; }
        public bool? IsCandidateVerificationCompleted { get; set; }
        public bool? IsJoiningLetterUploaded { get; set; }
        public bool? ShowOfferLetterActions { get; set; }
        /// <summary>Place of posting, date of joining, reporting time and a monthly CTC above zero are all filled in -
        /// required before the offer letter. Not returned by the SP; GetCandidateById fills it.</summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsJoiningDetailsComplete { get; set; }
        public bool? ShowAddAdditionalInfoButton { get; set; }
        public bool? ShowAppointmentLetterActions { get; set; }
        public bool? ShowJoiningLetterActions { get; set; }
        public bool? ShowWelcomeLetterActions { get; set; }
        public bool? IsEmployeeCreated { get; set; }
        public bool? ShowRejectButton { get; set; }

        public string? CurrentWorkflowStage { get; set; }
    }
}
