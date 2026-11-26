using QubeFin.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.Members.Models;

public class MemberSearchParam : SearchParam
{
    public int? BranchCode { get; set; }
    public Guid? GroupId { get; set; }
}
public class MemberSearchResult
{
    public Guid Id { get; set; }
    public int BranchCode { get; set; }
    public string? Code { get; set; }
    public string FullName { get; set; } = null!;
    public string? GuardianName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string MobileNo { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}
public record SearchTextRequest(string? SearchText);