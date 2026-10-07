using System.Text.Json.Serialization;

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
    public string? Address { get; private set; }
    public string? ReferenceNo { get; private set; }

    /// <summary>The day the candidate was created - set once, never edited.</summary>
    public DateOnly? ApplicationDate { get; private set; }

    /// <summary>Storage keys of the candidate's CV and job application. Both are mandatory at creation.</summary>
    public string? CvFile { get; private set; }
    public string? JobApplicationFile { get; private set; }

    /// <summary>HR picked the candidate for an offer after the HR Assessment. Candidate Verification opens only
    /// once this is set, and it is never reverted.</summary>
    public bool IsSelectedForOffer { get; private set; }

    /// <summary>HR has submitted (finalised) the HR Assessment. A saved draft leaves this false while
    /// RecommendationStatus moves off 'Pending'.</summary>
    public bool IsHrAssessmentCompleted { get; private set; }
    public DateOnly? InterviewDate { get; private set; }
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

    /// <summary>Storage key for the candidate's signed/returned joining letter, uploaded by HR once the
    /// candidate sends it back. Distinct from the SSRS-generated "Download Joining Letter" report, which is
    /// rendered on demand from candidate data rather than stored.</summary>
    public string? SignedJoiningLetterFile { get; private set; }
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
        DateOnly? interviewDate,
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
        string? signedJoiningLetterFile,
        Guid createdBy,
        DateTime createdOn,
        Guid? modifiedBy,
        DateTime? modifiedOn,
        string? address = null,
        DateOnly? applicationDate = null,
        string? cvFile = null,
        string? jobApplicationFile = null,
        bool isSelectedForOffer = false,
        bool isHrAssessmentCompleted = false)
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
        SignedJoiningLetterFile = signedJoiningLetterFile;
        ApplicationDate = applicationDate;
        CvFile = cvFile;
        JobApplicationFile = jobApplicationFile;
        IsSelectedForOffer = isSelectedForOffer;
        IsHrAssessmentCompleted = isHrAssessmentCompleted;

        Apply(firstName, middleName, lastName, gender, fatherName, mobileNo, email, houseNo, roadName, landMark,
            administrativeUnitId, policeStationId, postOfficeId, pinCode, interviewDate, interviewTime, departmentId,
            interviewPost, interviewPostName, venueOrganizationUnitId, interviewMode, referedBy, recruitmentSource, vacancyReference,
            currentSalary, expectedSalary, noticePeriodInDays, earliestJoiningDate, isWillingRelocate,
            preferredLocation, postedOrganizationUnitId, dateOfJoining, reportingTime, monthlyCostCompany,
            overallPerformance, suitableRoleDepartment, recommendedGradeId, isTrainingRequired, recommendationStatus,
            aadharNumber, voterNumber, pan, uan, address);
    }

    public static Candidate Create(Guid companyId, string referenceNo, Guid createdBy, CandidateDetails details, string cvFile, string jobApplicationFile)
    {
        return new Candidate
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            ReferenceNo = referenceNo,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow,
            ApplicationDate = DateOnly.FromDateTime(DateTime.Now),
            CvFile = cvFile,
            JobApplicationFile = jobApplicationFile,

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

    /// <summary>Adds or moves the candidate's interview slot (the Schedule action). Moving an existing slot makes the
    /// interview letter already sent out of date, so it is marked not sent - the Interview Letter actions come back
    /// for HR to send the new date. Returns whether the slot was moved.</summary>
    public bool Schedule(DateOnly interviewDate, TimeOnly interviewTime, Guid modifiedBy)
    {
        var isMoved = InterviewDate is not null && (InterviewDate != interviewDate || InterviewTime != interviewTime);

        InterviewDate = interviewDate;
        InterviewTime = interviewTime;
        if (isMoved)
        {
            IsInterviewLetterRecieved = false;
        }

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;

        return isMoved;
    }

    /// <summary>Replaces the CV and/or job application. A null key keeps the file already on record.</summary>
    public void SetDocuments(string? cvFile, string? jobApplicationFile, Guid modifiedBy)
    {
        CvFile = cvFile ?? CvFile;
        JobApplicationFile = jobApplicationFile ?? JobApplicationFile;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>HR selects the candidate for an offer. One-way: there is no un-select.</summary>
    public void SelectForOffer(Guid modifiedBy)
    {
        IsSelectedForOffer = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>The joining details the offer letter prints - place of posting, date of joining, reporting time and a
    /// monthly CTC above zero. HR must fill them in before the offer letter can be sent or marked received.</summary>
    public IReadOnlyList<string> MissingJoiningDetails()
    {
        var missing = new List<string>();
        if (PostedOrganizationUnitId is null) missing.Add("Place of Posting");
        if (DateOfJoining is null) missing.Add("Date of Joining");
        if (ReportingTime is null) missing.Add("Reporting Time");
        if (MonthlyCostCompany is not > 0) missing.Add("Monthly Cost to Company (CTC)");
        return missing;
    }

    /// <summary>Suffix HR's decision adds to a recommended candidate's RecommendationStatus when they are not selected.</summary>
    public const string NotSelectedSuffix = " but not selected";

    /// <summary>HR decides not to select a recommended candidate: IsSelectedForOffer stays false and the outcome is
    /// recorded on RecommendationStatus ("Recommended with Training but not selected"). That status no longer
    /// qualifies, so the workflow stops as Not Selected. One-way, like the selection.</summary>
    public void MarkNotSelected(Guid modifiedBy)
    {
        IsSelectedForOffer = false;
        RecommendationStatus = $"{RecommendationStatus}{NotSelectedSuffix}";
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

    /// <summary>Marks one verification check as verified and stores the value that was checked (document
    /// number, mobile number or credit bureau report link). A blank value keeps what is already on record.</summary>
    public void VerifyCheck(CandidateVerificationCheck check, string? value, Guid modifiedBy)
    {
        var trimmed = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        switch (check)
        {
            case CandidateVerificationCheck.Aadhar:
                AadharNumber = trimmed ?? AadharNumber;
                IsAadharValidated = true;
                break;
            case CandidateVerificationCheck.Voter:
                VoterNumber = trimmed ?? VoterNumber;
                IsVoterValited = true;
                break;
            case CandidateVerificationCheck.Pan:
                Pan = trimmed?.ToUpperInvariant() ?? Pan;
                IsPanValidated = true;
                break;
            case CandidateVerificationCheck.Mobile:
                MobileNo = trimmed ?? MobileNo;
                IsMobileValidated = true;
                break;
            case CandidateVerificationCheck.Uan:
                Uan = trimmed ?? Uan;
                IsUanVerified = true;
                break;
            case CandidateVerificationCheck.CreditBureau:
                CreditBureauReportLink = trimmed ?? CreditBureauReportLink;
                IsCreditBureauChecked = true;
                break;
        }

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

    /// <summary>Sets the uploaded signed/returned joining letter reference (storage key) for this candidate.</summary>
    public void SetSignedJoiningLetterFile(string? filePath, Guid modifiedBy)
    {
        SignedJoiningLetterFile = filePath;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>HR rejects the candidate: the workflow stops here for good. Recorded as the 'Rejected'
    /// RecommendationStatus (replacing whatever HR had recommended), with ModifiedBy/On as who and when.</summary>
    public void Reject(Guid rejectedBy)
    {
        RecommendationStatus = RejectedRecommendationStatus;
        ModifiedBy = rejectedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    public const string RejectedRecommendationStatus = "Rejected";

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
    /// come back to the draft. A draft is told apart by RecommendationStatus no longer being 'Pending' while
    /// IsHrAssessmentCompleted is still false.</summary>
    public void SaveHrAssessmentDraft(
        string? recommendationStatus,
        string? overallPerformance,
        string? suitableRoleDepartment,
        Guid? recommendedGradeId,
        bool isTrainingRequired,
        Guid modifiedBy)
    {
        RecommendationStatus = recommendationStatus;
        OverallPerformance = overallPerformance;
        SuitableRoleDepartment = suitableRoleDepartment;
        RecommendedGradeId = recommendedGradeId;
        IsTrainingRequired = isTrainingRequired;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Finalizes HR's assessment: sets IsHrAssessmentCompleted and stores the total and status derived
    /// from the average of the submitted interviewers' ratings - HR never types those in.</summary>
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
        IsHrAssessmentCompleted = true;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>The candidate's salary and joining expectations, as confirmed by HR on the HR Assessment
    /// form. Kept apart from the HR decision itself: these are facts about the candidate (also captured at
    /// candidate creation), not part of HR's recommendation, and both the draft and the submit write them.</summary>
    public void SaveSalaryAndJoiningDetails(
        decimal? currentSalary,
        decimal? expectedSalary,
        int? noticePeriodInDays,
        DateOnly? earliestJoiningDate,
        bool isWillingRelocate,
        string? preferredLocation,
        Guid modifiedBy)
    {
        CurrentSalary = currentSalary;
        ExpectedSalary = expectedSalary;
        NoticePeriodInDays = noticePeriodInDays;
        EarliestJoiningDate = earliestJoiningDate;
        IsWillingRelocate = isWillingRelocate;
        PreferredLocation = preferredLocation;

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
            details.AadharNumber, details.VoterNumber, details.Pan, details.Uan, details.Address);

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
        DateOnly? interviewDate,
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
        string? uan,
        string? address)
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
        Address = address;
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
    DateOnly? InterviewDate,
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
    string? Uan,
    string? Address = null);

/// <summary>A single candidate verification check, verified one at a time from the Candidate Verification form.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<CandidateVerificationCheck>))]
public enum CandidateVerificationCheck
{
    Aadhar,
    Voter,
    Pan,
    Mobile,
    Uan,
    CreditBureau,
}
