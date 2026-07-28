namespace QubeFin.Persistence.Models.Hrms;

public class LeaveType
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Alias { get; private set; } = null!;
    public bool IsPrayerable { get; private set; }
    public bool IsConvertible { get; private set; }
    public bool IsEncashable { get; private set; }
    public int NoOfDaysEntitled { get; private set; }
    public int? NoOfDaysCapped { get; private set; }
    public int MaxContinuousDays { get; private set; }
    public bool ApplicableAfterProbation { get; private set; }
    public bool IsMonthlyCredit { get; private set; }
    public int SeqNo { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }

    public LeaveType()
    {
        
    }
    public LeaveType(
        Guid id,
        string title,
        string alias,
        bool isPrayerable,
        bool isConvertible,
        bool isEncashable,
        int noOfDaysEntitled,
        int? noOfDaysCapped,
        int maxContinuousDays,
        bool applicableAfterProbation,
        bool isMonthlyCredit,
        int seqNo,
        Guid createdBy,
        DateTime createdOn,
        Guid? lastModifiedBy,
        DateTime? lastModifiedOn)
    {
        Id = id;
        Title = title;
        Alias = alias;
        IsPrayerable = isPrayerable;
        IsConvertible = isConvertible;
        IsEncashable = isEncashable;
        NoOfDaysEntitled = noOfDaysEntitled;
        NoOfDaysCapped = noOfDaysCapped;
        MaxContinuousDays = maxContinuousDays;
        ApplicableAfterProbation = applicableAfterProbation;
        IsMonthlyCredit = isMonthlyCredit;
        SeqNo = seqNo;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
    }

    public static LeaveType Create(Guid id,
        string title,
        string alias,
        bool isPrayerable,
        bool isConvertible,
        bool isEncashable,
        int noOfDaysEntitled,
        int? noOfDaysCapped,
        int maxContinuousDays,
        bool applicableAfterProbation,
        bool isMonthlyCredit,
        int seqNo,
        Guid createdBy)
    {
        return new LeaveType(id, title, alias, isPrayerable, isConvertible, isEncashable, noOfDaysEntitled, noOfDaysCapped, maxContinuousDays, applicableAfterProbation, isMonthlyCredit, seqNo, createdBy, DateTime.Now, null, null);
    }
}
