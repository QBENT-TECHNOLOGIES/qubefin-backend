using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblInterviewCandidate
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string? FatherName { get; set; }

    public string MobileNo { get; set; } = null!;

    public string? Email { get; set; }

    public string? HouseNo { get; set; }

    public string? RoadName { get; set; }

    public string? LandMark { get; set; }

    public Guid? AdministrativeUnitId { get; set; }

    public Guid? PoliceStationId { get; set; }

    public Guid? PostOfficeId { get; set; }

    public string? PinCode { get; set; }

    public string? ReferenceNo { get; set; }

    public DateOnly InterviewDate { get; set; }

    public TimeOnly? InterviewTime { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid InterviewPost { get; set; }

    public Guid? VenueOrganizationUnitId { get; set; }

    public string? InterviewMode { get; set; }

    public string? ReferedBy { get; set; }

    public decimal? CurrentSalary { get; set; }

    public decimal? ExpectedSalary { get; set; }

    public int? NoticePeriodInDays { get; set; }

    public DateOnly? EarliestJoiningDate { get; set; }

    public bool IsWillingRelocate { get; set; }

    public string? PreferredLocation { get; set; }

    public string? OverallPerformance { get; set; }

    public string? SuitableRoleDepartment { get; set; }

    public Guid? RecommendedGradeId { get; set; }

    public bool IsTrainingRequired { get; set; }

    public string? RecommendationStatus { get; set; }

    public int? TotalRatingPoint { get; set; }

    public string? RatingStatus { get; set; }

    public string? RecruitmentSource { get; set; }

    public string? VacancyReference { get; set; }

    public string? AadharNumber { get; set; }

    public bool IsAadharValidated { get; set; }

    public string? VoterNumber { get; set; }

    public bool IsVoterValited { get; set; }

    public string? Pan { get; set; }

    public bool IsPanValidated { get; set; }

    public bool IsMobileValidated { get; set; }

    public string? Uan { get; set; }

    public bool IsUanVerified { get; set; }

    public bool IsCreditBureauChecked { get; set; }

    public string? CreditBureauReportLink { get; set; }

    public Guid? PostedOrganizationUnitId { get; set; }

    public DateOnly? DateOfJoining { get; set; }

    public TimeOnly? ReportingTime { get; set; }

    public decimal? MonthlyCostCompany { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblAdministrativeUnit? AdministrativeUnit { get; set; }

    public virtual TblCompany Company { get; set; } = null!;

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblDepartment? Department { get; set; }

    public virtual TblUser? ModifiedByNavigation { get; set; }

    public virtual TblPoliceStation? PoliceStation { get; set; }

    public virtual TblPostOffice? PostOffice { get; set; }

    public virtual TblOrganizationUnit? PostedOrganizationUnit { get; set; }

    public virtual ICollection<TblInterviewPanel> TblInterviewPanels { get; set; } = new List<TblInterviewPanel>();

    public virtual TblOrganizationUnit? VenueOrganizationUnit { get; set; }
}
