namespace QubeFin.Persistence.Models.App;

public class UserSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string SessionToken { get; private set; } = null!;
    public bool IsMfaVerified { get; private set; }
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime LoginTime { get; private set; }
    public DateTime? LogoutTime { get; private set; }

    private UserSession() { }

    public UserSession(Guid id, Guid userId, string sessionToken, bool isMfaVerified, string? accessToken, string? refreshToken, DateTime loginTime, DateTime? logoutTime)
    {
        Id = id;
        UserId = userId;
        SessionToken = sessionToken;
        IsMfaVerified = isMfaVerified;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        LoginTime = loginTime;
        LogoutTime = logoutTime;
    }

    public static UserSession Create(Guid id, Guid userId, string sessionToken)
    {
        var userSession = new UserSession
        {
            Id = id,
            UserId = userId,
            SessionToken = sessionToken,
            IsMfaVerified = false,
            LoginTime = DateTime.Now
        };

        return userSession;
    }

    public void UpdateSessionToken(string sessionToken) => SessionToken = sessionToken;

    public void Open(string accessToken, string refreshToken)
    {
        LoginTime = DateTime.Now;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public void Close()
    {
        LogoutTime = DateTime.Now;
    }
}
