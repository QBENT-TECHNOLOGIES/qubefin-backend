namespace QubeFin.Persistence.Models.Tracking;

// One branch of Global.Fn_GetUserWiseBranches, with its code
public class UserWiseBranchItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}

// One row of Global.USP_GetBranchWiseCreditOfficers
public class BranchCreditOfficerItem
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public string Name { get; set; } = null!;
}
