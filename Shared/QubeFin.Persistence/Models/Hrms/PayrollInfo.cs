namespace QubeFin.Persistence.Models.Hrms;

public class PayrollInfo
{
    public Guid? BankId { get; private set; }
    public long? BankAccountNo { get; private set; }
    public string? BankHolderName { get; private set; }
    public string? BankBranch { get; private set; }
    public string? BankAccountType { get; private set; }
    public bool HasEsiEligible { get; private set; }
    public string? EsiIpNumber { get; private set; }
    public string? UniversalAccountNumber { get; private set; }
    public bool IsPayrollActive { get; private set; }

    public PayrollInfo()
    {
        
    }

    public PayrollInfo(Guid? bankId, long? bankAccountNo, string? bankHolderName, string? bankBranch, string? bankAccountType, 
        bool hasEsiEligible, string? esiIpNumber, string? universalAccountNumber, bool isPayrollActive)
    {
        BankId = bankId;
        BankAccountNo = bankAccountNo;
        BankHolderName = bankHolderName;
        BankBranch = bankBranch;
        BankAccountType = bankAccountType;
        HasEsiEligible = hasEsiEligible;
        EsiIpNumber = esiIpNumber;
        UniversalAccountNumber = universalAccountNumber;
        IsPayrollActive = isPayrollActive;
    }
}
