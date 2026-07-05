using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeReference
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string PersonName { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Occupation { get; set; }

    public string? HowDoYouKnow { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;
}
