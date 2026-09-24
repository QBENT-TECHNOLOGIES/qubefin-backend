using System;

namespace QubeFin.Hrms.Application.Employees.Models;

public class ReferralInfoRequest
{
    public Guid? ReferedBy { get; set; }
    public string? HowYouKnow { get; set; }
}
