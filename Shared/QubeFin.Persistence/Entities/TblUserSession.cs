using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblUserSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string SessionToken { get; set; } = null!;

    public bool IsMfaVerified { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string? DeviceId { get; set; }

    public string? UserAgent { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
