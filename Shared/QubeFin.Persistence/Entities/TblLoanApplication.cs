using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanApplication
{
    public Guid Id { get; set; }

    public string ApplicationNo { get; set; } = null!;

    public DateOnly ApplicationDate { get; set; }

    public Guid BorrowerId { get; set; }

    public Guid? BorrowerAddressId { get; set; }

    public Guid? BorrowerCreditDataId { get; set; }

    public Guid? CoBorrowerId { get; set; }

    public Guid? CoBorrowerAddressId { get; set; }

    public Guid? CoBorrowerCreditDataId { get; set; }

    public string? BorCoBorRelation { get; set; }

    public Guid LoanProductId { get; set; }

    public Guid? LoanPurposeId { get; set; }

    public Guid? LoanSubPurposeId { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public Guid GroupId { get; set; }

    public Guid? DesignationId { get; set; }

    public decimal? ApplicationAmount { get; set; }

    public decimal? AprovedAmount { get; set; }

    public decimal? ApprovedFlatRate { get; set; }

    public decimal? ApprovedInterestRate { get; set; }

    public int? ApprovedInstallmentPeriod { get; set; }

    public int? ApprovedFactor { get; set; }

    public decimal? InstallmentInYear { get; set; }

    public string? PaymentSchedule { get; set; }

    public bool IsEcsLoan { get; set; }

    public bool IsCancelled { get; set; }

    public string? Remarks { get; set; }

    public bool IsVerifiedByAcUser { get; set; }

    public bool BpiChargeApplied { get; set; }

    public Guid? CompanyAccountId { get; set; }

    public int? DownloadFileSeq { get; set; }

    public string? DownloadFileName { get; set; }

    public bool IsDisbursed { get; set; }

    public DateTime? DisbursedOn { get; set; }

    public decimal? DisbursedAmount { get; set; }

    public string? Neftno { get; set; }

    public decimal? AdjustedAmount { get; set; }

    public decimal? DocumentCharge { get; set; }

    public decimal? BpiAmount { get; set; }

    public decimal? LoginFees { get; set; }

    public decimal? ProcessingFeePercent { get; set; }

    public decimal? ProcessingFeeAmount { get; set; }

    public decimal? ProcessingFeeCgstpercent { get; set; }

    public decimal? ProcessingFeeCgstamount { get; set; }

    public decimal? ProcessingFeeSgstpercent { get; set; }

    public decimal? ProcessingFeeSgstamount { get; set; }

    public decimal? ProcessingFeeIgstpercent { get; set; }

    public decimal? ProcessingFeeIgstamount { get; set; }

    public decimal? InspectionChargePercent { get; set; }

    public decimal? InspectionChargeAmount { get; set; }

    public decimal? MortalityCoveragePercent { get; set; }

    public decimal? MortalityCoverageAmount { get; set; }

    public decimal? CersaiCharge { get; set; }

    public decimal? CersaiCgstpercent { get; set; }

    public decimal? CersaiCgstamount { get; set; }

    public decimal? CersaiSgstpercent { get; set; }

    public decimal? CersaiSgstamount { get; set; }

    public decimal? TotalMortalityAmount { get; set; }

    public decimal? TotalCharges { get; set; }

    public decimal? ProposedEmi { get; set; }

    public string? UdyamRegNo { get; set; }

    public DateOnly? UdyamRegDate { get; set; }

    public bool IsHold { get; set; }

    public string? EnachComplete { get; set; }

    public bool CoBorrowerMortalityApplied { get; set; }

    public DateTime? LoanApprovedDate { get; set; }

    public DateOnly? LockingDate { get; set; }

    public string? OngikarnamaPhoto { get; set; }

    public bool IsDeviationApproved { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblMember Borrower { get; set; } = null!;

    public virtual TblMemberAddress? BorrowerAddress { get; set; }

    public virtual TblCreditDatum? BorrowerCreditData { get; set; }

    public virtual TblCoBorrower? CoBorrower { get; set; }

    public virtual TblMemberAddress? CoBorrowerAddress { get; set; }

    public virtual TblCreditDatum? CoBorrowerCreditData { get; set; }

    public virtual TblCompanyBankAccount? CompanyAccount { get; set; }

    public virtual TblDesignation? Designation { get; set; }

    public virtual TblGroup Group { get; set; } = null!;

    public virtual TblLoanProduct LoanProduct { get; set; } = null!;

    public virtual TblOrganizationUnit OrganizationUnit { get; set; } = null!;

    public virtual ICollection<TblLoanApplicationWorkflow> TblLoanApplicationWorkflows { get; set; } = new List<TblLoanApplicationWorkflow>();
}
