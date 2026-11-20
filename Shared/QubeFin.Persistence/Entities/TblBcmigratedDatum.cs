using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcmigratedDatum
{
    public Guid Id { get; set; }

    public string? TerritoryId { get; set; }

    public string? TerritoryName { get; set; }

    public string? BranchId { get; set; }

    public string? BranchName { get; set; }

    public string? CreditOfficerId { get; set; }

    public string? CreditOfficerName { get; set; }

    public string? UnitName { get; set; }

    public string? GroupId { get; set; }

    public string? GroupName { get; set; }

    public string? SubGroupName { get; set; }

    public string? ClientId { get; set; }

    public string? ClientName { get; set; }

    public string? MobileNo { get; set; }

    public DateOnly? ClientJoinDate { get; set; }

    public string? AccountId { get; set; }

    public string? LoanSeries { get; set; }

    public string? ProductId { get; set; }

    public string? SectorName { get; set; }

    public string? LoanPurpose { get; set; }

    public DateOnly? DisbursementDate { get; set; }

    public decimal? LoanAmount { get; set; }

    public decimal? InterestAmount { get; set; }

    public int? CurrentInstallmentNo { get; set; }

    public decimal? PrincipalDue { get; set; }

    public decimal? InterestDue { get; set; }

    public decimal? TotalDue { get; set; }

    public decimal? PrincipalPaid { get; set; }

    public decimal? InterestPaid { get; set; }

    public decimal? TotalPaid { get; set; }

    public decimal? PrincipalPrePaid { get; set; }

    public decimal? InterestPrePaid { get; set; }

    public decimal? TotalPrePaid { get; set; }

    public decimal? TotalAdvancePaid { get; set; }

    public decimal? PrincipalArrear { get; set; }

    public decimal? InterestArrear { get; set; }

    public decimal? TotalArrear { get; set; }

    public int? NoOfInstallmentDue { get; set; }

    public int? DueDays { get; set; }

    public string? AgingOfOverdue { get; set; }

    public decimal? PrincipalOutstanding { get; set; }

    public decimal? InterestOutstanding { get; set; }

    public decimal? TotalOutstading { get; set; }

    public string? LoanStatus { get; set; }

    public string? InstallmentFrequency { get; set; }

    public DateOnly? LoanMaturityDate { get; set; }

    public string? FunderId { get; set; }

    public Guid? OurganizationUnitId { get; set; }

    public Guid? GoupMasterId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? MemberId { get; set; }

    public Guid? LoanId { get; set; }

    public DateTime UploadDate { get; set; }
}
