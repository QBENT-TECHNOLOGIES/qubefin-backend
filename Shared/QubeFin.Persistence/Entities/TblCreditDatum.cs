using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDatum
{
    public Guid Id { get; set; }

    public Guid? MemberId { get; set; }

    public Guid? CoBorrowerId { get; set; }

    public Guid CreditOrganizationId { get; set; }

    public string? OrganizationName { get; set; }

    public string? ConsumerName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Age { get; set; }

    public int? Score { get; set; }

    public int? MfiScore { get; set; }

    public int? TotalAcc { get; set; }

    public int? NoOfActiveAcc { get; set; }

    public int? NoOfClosedAcc { get; set; }

    public int? NoOfPastDueAcc { get; set; }

    public int? NoOfWrittenOffAcc { get; set; }

    public decimal? TotalSanctionedAmount { get; set; }

    public decimal? TotalBalanceAmount { get; set; }

    public decimal? TotalMonthlyInstAmount { get; set; }

    public decimal? TotalPastDueAmount { get; set; }

    public decimal? TotalWritoffAmount { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? DocumentId { get; set; }

    public string? DocFilename { get; set; }

    public string? XmlDocFileName { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblCoBorrower? CoBorrower { get; set; }

    public virtual TblCreditOrganization CreditOrganization { get; set; } = null!;

    public virtual TblMember? Member { get; set; }

    public virtual ICollection<TblCreditDataAccount> TblCreditDataAccounts { get; set; } = new List<TblCreditDataAccount>();

    public virtual ICollection<TblCreditDataAddress> TblCreditDataAddresses { get; set; } = new List<TblCreditDataAddress>();

    public virtual ICollection<TblCreditDataAlert> TblCreditDataAlerts { get; set; } = new List<TblCreditDataAlert>();

    public virtual ICollection<TblCreditDataDependent> TblCreditDataDependents { get; set; } = new List<TblCreditDataDependent>();

    public virtual ICollection<TblCreditDataIdentity> TblCreditDataIdentities { get; set; } = new List<TblCreditDataIdentity>();

    public virtual ICollection<TblCreditDataIncome> TblCreditDataIncomes { get; set; } = new List<TblCreditDataIncome>();
}
