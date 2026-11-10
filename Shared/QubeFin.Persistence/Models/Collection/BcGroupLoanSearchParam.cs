namespace QubeFin.Persistence.Models.Collection;

/// <summary>Search, sort and paging for the group wise BC loan list.</summary>
public class BcGroupLoanSearchParam : SearchParam
{
    /// <summary>Null = both, true = closed loans only, false = running loans only.</summary>
    public bool? IsClosed { get; set; }
}
