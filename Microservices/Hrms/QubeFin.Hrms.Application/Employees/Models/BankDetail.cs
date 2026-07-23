namespace QubeFin.Hrms.Application.Employees.Models;

public class BankDetail
{
    public Guid? BankId { get;  set; }
    public long BankAccountNo { get;  set; }
    public string BankHolderName { get;  set; }
    public string? BankBranch { get;  set; }
    public string? BankAccountType { get;  set; }
    public bool HasEsiEligible { get;  set; }
    public string? EsiIpNumber { get;  set; }
    public string? UniversalAccountNumber { get;  set; }
    public bool IsPayrollActive { get;  set; }
}
