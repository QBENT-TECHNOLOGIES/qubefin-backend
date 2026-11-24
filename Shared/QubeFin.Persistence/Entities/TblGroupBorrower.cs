using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGroupBorrower
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public string Name { get; set; } = null!;

    public string GuardianName { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string MaritalStatus { get; set; } = null!;

    public int Age { get; set; }

    public bool OwnHouse { get; set; }

    public string? IncomeActivity { get; set; }

    public bool YearlyIncome { get; set; }

    public bool Indebtedness { get; set; }

    public bool HasNpa { get; set; }

    public bool HasWriteOff { get; set; }

    public bool IsFlagged { get; set; }

    public string? FlagReasons { get; set; }

    public string? DecisionReason { get; set; }

    public string Status { get; set; } = null!;

    public string? RejectReason { get; set; }

    public Guid? MemberId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public decimal Income { get; set; }

    public decimal LoanAmount { get; set; }

    public Guid? GroupId { get; set; }

    public decimal? ProposedLoanAmount { get; set; }

    public string? LoanPurpose { get; set; }

    public string? EligibleRemarks { get; set; }

    public string? GroupRole { get; set; }

    public DateOnly? AppointmentDate { get; set; }

    public virtual TblGroup? Group { get; set; }

    public virtual TblMember? Member { get; set; }

    public virtual TblSurvey Survey { get; set; } = null!;
}
