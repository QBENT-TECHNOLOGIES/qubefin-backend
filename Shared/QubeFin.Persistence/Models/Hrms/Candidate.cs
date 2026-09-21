namespace QubeFin.Persistence.Models.Hrms;

public class Candidate
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public string Gender { get; private set; } = string.Empty;
    public string? FatherName { get; private set; }
    public string MobileNo { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? HouseNo { get; private set; }
    public string? RoadName { get; private set; }
    public string? LandMark { get; private set; }
    public Guid? AdministrativeUnitId { get; private set; }
    public Guid? PoliceStationId { get; private set; }
    public Guid? PostOfficeId { get; private set; }
    public string? PinCode { get; private set; }
    public string? ReferenceNo { get; private set; }
    public DateOnly InterviewDate { get; private set; }
    public TimeOnly? InterviewTime { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid InterviewPost { get; private set; }
    public string InterviewPostName { get; private set; }
    public Guid? VenueOrganizationUnitId { get; private set; }
    public string? InterviewMode { get; private set; }
    public string? ReferedBy { get; private set; }
    public string? RecruitmentSource { get; private set; }
    public string? VacancyReference { get; private set; }
    public decimal? CurrentSalary { get; private set; }
    public decimal? ExpectedSalary { get; private set; }
    public int? NoticePeriodInDays { get; private set; }
    public DateOnly? EarliestJoiningDate { get; private set; }
    public bool IsWillingRelocate { get; private set; }
    public string? PreferredLocation { get; private set; }
    public Guid? PostedOrganizationUnitId { get; private set; }
    public DateOnly? DateOfJoining { get; private set; }
    public TimeOnly? ReportingTime { get; private set; }
    public decimal? MonthlyCostCompany { get; private set; }
    public string? OverallPerformance { get; private set; }
    public string? SuitableRoleDepartment { get; private set; }
    public Guid? RecommendedGradeId { get; private set; }
    public bool IsTrainingRequired { get; private set; }
    public string? RecommendationStatus { get; private set; }
    public int? TotalRatingPoint { get; private set; }
    public string? RatingStatus { get; private set; }
    public string? AadharNumber { get; private set; }
    public bool IsAadharValidated { get; private set; }
    public string? VoterNumber { get; private set; }
    public bool IsVoterValited { get; private set; }
    public string? Pan { get; private set; }
    public bool IsPanValidated { get; private set; }
    public bool IsMobileValidated { get; private set; }
    public string? Uan { get; private set; }
    public bool IsUanVerified { get; private set; }
    public bool IsCreditBureauChecked { get; private set; }
    public string? CreditBureauReportLink { get; private set; }
    public bool IsInterviewLetterRecieved { get; private set; }
    public bool IsOfferLetterReceived { get; private set; }
    public bool IsAppointmentLetterReceived { get; private set; }
    public bool IsWelcomeLetterRecieved { get; private set; }
    public string? WrittenInterviewFile { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? ModifiedBy { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    private Candidate() { }

    public Candidate(
        Guid id,
        Guid companyId,
        string firstName,
        string? middleName,
        string lastName,
        string gender,
        string? fatherName,
        string mobileNo,
        string? email,
        string? houseNo,
        string? roadName,
        string? landMark,
        Guid? administrativeUnitId,
        Guid? policeStationId,
        Guid? postOfficeId,
        string? pinCode,
        string? referenceNo,
        DateOnly interviewDate,
        TimeOnly? interviewTime,
        Guid? departmentId,
        Guid interviewPost,
        string? interviewPostName,
        Guid? venueOrganizationUnitId,
        string? interviewMode,
        string? referedBy,
        string? recruitmentSource,
        string? vacancyReference,
        decimal? currentSalary,
        decimal? expectedSalary,
        int? noticePeriodInDays,
        DateOnly? earliestJoiningDate,
        bool isWillingRelocate,
        string? preferredLocation,
        Guid? postedOrganizationUnitId,
        DateOnly? dateOfJoining,
        TimeOnly? reportingTime,
        decimal? monthlyCostCompany,
        string? overallPerformance,
        string? suitableRoleDepartment,
        Guid? recommendedGradeId,
        bool isTrainingRequired,
        string? recommendationStatus,
        int? totalRatingPoint,
        string? ratingStatus,
        string? aadharNumber,
        bool isAadharValidated,
        string? voterNumber,
        bool isVoterValited,
        string? pan,
        bool isPanValidated,
        bool isMobileValidated,
        string? uan,
        bool isUanVerified,
        bool isCreditBureauChecked,
        string? creditBureauReportLink,
        bool isInterviewLetterRecieved,
        bool isOfferLetterReceived,
        bool isAppointmentLetterReceived,
        bool isWelcomeLetterRecieved,
        string? writtenInterviewFile,
        Guid createdBy,
        DateTime createdOn,
        Guid? modifiedBy,
        DateTime? modifiedOn)
    {
        Id = id;
        CompanyId = companyId;
        ReferenceNo = referenceNo;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        ModifiedBy = modifiedBy;
        ModifiedOn = modifiedOn;
        TotalRatingPoint = totalRatingPoint;
        RatingStatus = ratingStatus;
        IsAadharValidated = isAadharValidated;
        IsVoterValited = isVoterValited;
        IsPanValidated = isPanValidated;
        IsMobileValidated = isMobileValidated;
        IsUanVerified = isUanVerified;
        IsCreditBureauChecked = isCreditBureauChecked;
        CreditBureauReportLink = creditBureauReportLink;
        IsInterviewLetterRecieved = isInterviewLetterRecieved;
        IsOfferLetterReceived = isOfferLetterReceived;
        IsAppointmentLetterReceived = isAppointmentLetterReceived;
        IsWelcomeLetterRecieved = isWelcomeLetterRecieved;
        WrittenInterviewFile = writtenInterviewFile;

        Apply(firstName, middleName, lastName, gender, fatherName, mobileNo, email, houseNo, roadName, landMark,
            administrativeUnitId, policeStationId, postOfficeId, pinCode, interviewDate, interviewTime, departmentId,
            interviewPost, interviewPostName, venueOrganizationUnitId, interviewMode, referedBy, recruitmentSource, vacancyReference,
            currentSalary, expectedSalary, noticePeriodInDays, earliestJoiningDate, isWillingRelocate,
            preferredLocation, postedOrganizationUnitId, dateOfJoining, reportingTime, monthlyCostCompany,
            overallPerformance, suitableRoleDepartment, recommendedGradeId, isTrainingRequired, recommendationStatus,
            aadharNumber, voterNumber, pan, uan);
    }

    public static Candidate Create(Guid companyId, string referenceNo, Guid createdBy, CandidateDetails details)
    {
        return new Candidate
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            ReferenceNo = referenceNo,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow,

            // Initial values
            RecommendationStatus = "Pending",
            RatingStatus = "Not started"
        }.Apply(details);
    }

    public void Update(CandidateDetails details, Guid modifiedBy)
    {
        Apply(details);
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Updates one letter-received flag at a time (only the flag(s) passed as non-null are changed).</summary>
    public void UpdateLetterStatus(
        bool? isInterviewLetterReceived,
        bool? isOfferLetterReceived,
        bool? isAppointmentLetterReceived,
        bool? isWelcomeLetterReceived,
        Guid modifiedBy)
    {
        if (isInterviewLetterReceived.HasValue)
        {
            IsInterviewLetterRecieved = isInterviewLetterReceived.Value;
        }

        if (isOfferLetterReceived.HasValue)
        {
            IsOfferLetterReceived = isOfferLetterReceived.Value;
        }

        if (isAppointmentLetterReceived.HasValue)
        {
            IsAppointmentLetterReceived = isAppointmentLetterReceived.Value;
        }

        if (isWelcomeLetterReceived.HasValue)
        {
            IsWelcomeLetterRecieved = isWelcomeLetterReceived.Value;
        }

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Records the outcome of the candidate's background/identity verification checks. All flags are
    /// set together (unlike <see cref="UpdateLetterStatus"/>, which updates one at a time).</summary>
    public void UpdateVerification(
        bool isAadharValidated,
        bool isVoterValited,
        bool isPanValidated,
        bool isMobileValidated,
        bool isUanVerified,
        bool isCreditBureauChecked,
        string? creditBureauReportLink,
        Guid modifiedBy)
    {
        IsAadharValidated = isAadharValidated;
        IsVoterValited = isVoterValited;
        IsPanValidated = isPanValidated;
        IsMobileValidated = isMobileValidated;
        IsUanVerified = isUanVerified;
        IsCreditBureauChecked = isCreditBureauChecked;
        CreditBureauReportLink = creditBureauReportLink;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Sets the uploaded written-interview form/document reference (storage key) for this candidate.</summary>
    public void SetWrittenInterviewFile(string? filePath, Guid modifiedBy)
    {
        WrittenInterviewFile = filePath;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Sets whether the interview was conducted Online or Offline. Editable on its own (e.g. from the
    /// HR Assessment form) rather than only via the full candidate update.</summary>
    public void SetInterviewMode(string interviewMode, Guid modifiedBy)
    {
        InterviewMode = interviewMode;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Stores HR's in-progress assessment decision. Deliberately leaves RecommendationStatus,
    /// TotalRatingPoint and RatingStatus untouched: the workflow reads RecommendationStatus = 'Pending' as
    /// "HR Assessment not finished yet", and that is what keeps the HR Assessment button visible so HR can
    /// come back to the draft. The averaged category ratings live on HR's own Tbl_InterviewPanel row.</summary>
    public void SaveHrAssessmentDraft(
        string? overallPerformance,
        string? suitableRoleDepartment,
        Guid? recommendedGradeId,
        bool isTrainingRequired,
        Guid modifiedBy)
    {
        OverallPerformance = overallPerformance;
        SuitableRoleDepartment = suitableRoleDepartment;
        RecommendedGradeId = recommendedGradeId;
        IsTrainingRequired = isTrainingRequired;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Finalizes HR's assessment. Unlike the draft this also writes RecommendationStatus, which moves
    /// the candidate out of the HR_ASSESSMENT stage, plus the total and status derived from the average of the
    /// interviewers' ratings - HR never types those in, they are computed from Tbl_InterviewPanel.</summary>
    public void SubmitHrAssessment(
        string? overallPerformance,
        string? suitableRoleDepartment,
        Guid? recommendedGradeId,
        bool isTrainingRequired,
        string recommendationStatus,
        int? totalRatingPoint,
        string? ratingStatus,
        Guid modifiedBy)
    {
        OverallPerformance = overallPerformance;
        SuitableRoleDepartment = suitableRoleDepartment;
        RecommendedGradeId = recommendedGradeId;
        IsTrainingRequired = isTrainingRequired;
        RecommendationStatus = recommendationStatus;
        TotalRatingPoint = totalRatingPoint;
        RatingStatus = ratingStatus;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    private Candidate Apply(CandidateDetails details)
    {
        Apply(details.FirstName, details.MiddleName, details.LastName, details.Gender, details.FatherName,
            details.MobileNo, details.Email, details.HouseNo, details.RoadName, details.LandMark,
            details.AdministrativeUnitId, details.PoliceStationId, details.PostOfficeId, details.PinCode,
            details.InterviewDate, details.InterviewTime, details.DepartmentId, details.InterviewPost, details.InterviewPostName,
            details.VenueOrganizationUnitId, details.InterviewMode, details.ReferedBy, details.RecruitmentSource,
            details.VacancyReference, details.CurrentSalary, details.ExpectedSalary, details.NoticePeriodInDays,
            details.EarliestJoiningDate, details.IsWillingRelocate, details.PreferredLocation,
            details.PostedOrganizationUnitId, details.DateOfJoining, details.ReportingTime,
            details.MonthlyCostCompany, details.OverallPerformance, details.SuitableRoleDepartment,
            details.RecommendedGradeId, details.IsTrainingRequired, details.RecommendationStatus,
            details.AadharNumber, details.VoterNumber, details.Pan, details.Uan);

        return this;
    }

    private void Apply(
        string firstName,
        string? middleName,
        string lastName,
        string gender,
        string? fatherName,
        string mobileNo,
        string? email,
        string? houseNo,
        string? roadName,
        string? landMark,
        Guid? administrativeUnitId,
        Guid? policeStationId,
        Guid? postOfficeId,
        string? pinCode,
        DateOnly interviewDate,
        TimeOnly? interviewTime,
        Guid? departmentId,
        Guid interviewPost,
        string? interviewPostName,
        Guid? venueOrganizationUnitId,
        string? interviewMode,
        string? referedBy,
        string? recruitmentSource,
        string? vacancyReference,
        decimal? currentSalary,
        decimal? expectedSalary,
        int? noticePeriodInDays,
        DateOnly? earliestJoiningDate,
        bool isWillingRelocate,
        string? preferredLocation,
        Guid? postedOrganizationUnitId,
        DateOnly? dateOfJoining,
        TimeOnly? reportingTime,
        decimal? monthlyCostCompany,
        string? overallPerformance,
        string? suitableRoleDepartment,
        Guid? recommendedGradeId,
        bool isTrainingRequired,
        string? recommendationStatus,
        string? aadharNumber,
        string? voterNumber,
        string? pan,
        string? uan)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Gender = gender;
        FatherName = fatherName;
        MobileNo = mobileNo;
        Email = email;
        HouseNo = houseNo;
        RoadName = roadName;
        LandMark = landMark;
        AdministrativeUnitId = administrativeUnitId;
        PoliceStationId = policeStationId;
        PostOfficeId = postOfficeId;
        PinCode = pinCode;
        InterviewDate = interviewDate;
        InterviewTime = interviewTime;
        DepartmentId = departmentId;
        InterviewPost = interviewPost;
        InterviewPostName = interviewPostName;
        VenueOrganizationUnitId = venueOrganizationUnitId;
        InterviewMode = interviewMode;
        ReferedBy = referedBy;
        RecruitmentSource = recruitmentSource;
        VacancyReference = vacancyReference;
        CurrentSalary = currentSalary;
        ExpectedSalary = expectedSalary;
        NoticePeriodInDays = noticePeriodInDays;
        EarliestJoiningDate = earliestJoiningDate;
        IsWillingRelocate = isWillingRelocate;
        PreferredLocation = preferredLocation;
        PostedOrganizationUnitId = postedOrganizationUnitId;
        DateOfJoining = dateOfJoining;
        ReportingTime = reportingTime;
        MonthlyCostCompany = monthlyCostCompany;
        OverallPerformance = overallPerformance;
        SuitableRoleDepartment = suitableRoleDepartment;
        RecommendedGradeId = recommendedGradeId;
        IsTrainingRequired = isTrainingRequired;
        if (!string.IsNullOrWhiteSpace(recommendationStatus))
        {
            RecommendationStatus = recommendationStatus;
        }
        AadharNumber = aadharNumber;
        VoterNumber = voterNumber;
        Pan = pan;
        Uan = uan;
    }
}

public record CandidateDetails(
    string FirstName,
    string? MiddleName,
    string LastName,
    string Gender,
    string? FatherName,
    string MobileNo,
    string? Email,
    string? HouseNo,
    string? RoadName,
    string? LandMark,
    Guid? AdministrativeUnitId,
    Guid? PoliceStationId,
    Guid? PostOfficeId,
    string? PinCode,
    DateOnly InterviewDate,
    TimeOnly? InterviewTime,
    Guid? DepartmentId,
    Guid InterviewPost,
    string? InterviewPostName,
    Guid? VenueOrganizationUnitId,
    string? InterviewMode,
    string? ReferedBy,
    string? RecruitmentSource,
    string? VacancyReference,
    decimal? CurrentSalary,
    decimal? ExpectedSalary,
    int? NoticePeriodInDays,
    DateOnly? EarliestJoiningDate,
    bool IsWillingRelocate,
    string? PreferredLocation,
    Guid? PostedOrganizationUnitId,
    DateOnly? DateOfJoining,
    TimeOnly? ReportingTime,
    decimal? MonthlyCostCompany,
    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,
    string? AadharNumber,
    string? VoterNumber,
    string? Pan,
    string? Uan);
