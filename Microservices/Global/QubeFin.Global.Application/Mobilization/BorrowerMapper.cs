using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.Mobilization
{
    public static class BorrowerMapper
    {
        public static BorrowerItem ToItem(TblGroupBorrower b) => new()
        {
            Id = b.Id,
            Name = b.Name,
            GuardianName = b.GuardianName,
            MobileNo = b.MobileNo,
            MaritalStatus = b.MaritalStatus,
            Age = b.Age,
            OwnHouse = b.OwnHouse,
            IncomeActivity = b.IncomeActivity,
            Income = b.Income,
            LoanAmount = b.LoanAmount,
            IncomeWithinLimit = b.YearlyIncome,
            IndebtednessWithinLimit = b.Indebtedness,
            HasNpa = b.HasNpa,
            HasWriteOff = b.HasWriteOff,
            IsFlagged = b.IsFlagged,
            FlagReasons = b.FlagReasons,
            DecisionReason = b.DecisionReason,
            Status = b.Status,
            RejectReason = b.RejectReason,
            // A borrower who joined a group can no longer be changed here.
            CanChange = b.GroupId == null && (b.Status == BorrowerStatuses.Pending || b.Status == BorrowerStatuses.Blocked)
        };
    }
}
