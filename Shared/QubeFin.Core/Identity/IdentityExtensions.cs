using System.Security.Claims;
using System.Security.Principal;

namespace QubeFin.Core.Identity;

public static class IdentityExtensions
{
    public static Guid GetUserId(this IIdentity identity)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        var userIdClaim = claimsIdentity?.FindFirst("UserId");
        return userIdClaim == null ? Guid.Empty : Guid.Parse(userIdClaim.Value!.ToString());
    }

    public static Guid GetEmployeeId(this IIdentity identity)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        var employeeIdClaim = claimsIdentity?.FindFirst("EmployeeId");
        return employeeIdClaim == null ? Guid.Empty : Guid.Parse(employeeIdClaim.Value!.ToString());
    }
}
