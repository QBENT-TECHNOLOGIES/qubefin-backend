namespace QubeFin.App.Api.Requests
{
    public class UserUpdateRequest
    {
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool HasMfaEnabled { get; set; }
    }
}
