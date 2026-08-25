namespace QubeFin.Persistence.Models.Hrms;

public class PayrollInfo
{
    public Guid? BankId { get; private set; }
    public string? BankHolderName { get; private set; }
    public long? BankAccountNo { get; private set; }
    public string? IfscCode { get; set; }
    public string? BankBranch { get; private set; }
    public string? BankAccountType { get; private set; }

    public string? UniversalAccountNumber { get; private set; }
    public string? PFAccountNo { get; private set; }
    public bool HasEsiEligible { get; private set; }
    public string? EsiIpNumber { get; private set; }
    public bool IsPayrollActive { get; private set; }

    public PayrollInfo()
    {

    }

    public PayrollInfo(Guid? bankId, string? bankHolderName, long? bankAccountNo, string? ifscCode, string? bankBranch, string? bankAccountType,
        string? universalAccountNumber, string? pFAccountNo, bool hasEsiEligible, string? esiIpNumber, bool isPayrollActive)
    {
        BankId = bankId;
        BankHolderName = bankHolderName;
        BankAccountNo = bankAccountNo;
        IfscCode = ifscCode;
        BankBranch = bankBranch;
        BankAccountType = bankAccountType;

        UniversalAccountNumber = universalAccountNumber;
        PFAccountNo = pFAccountNo;
        HasEsiEligible = hasEsiEligible;
        EsiIpNumber = esiIpNumber;
        IsPayrollActive = isPayrollActive;
    }
}
