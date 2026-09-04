namespace QubeFin.Auth.Application.Accounts.Model
{
    public class ForgotPasswordInitiateRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public string? UserAgent { get; set; }
    }
    public class ForgotPasswordVerifyMfaRequest
    {
        public string MfaCode { get; set; } = string.Empty;
        public string SessionToken { get; set; } = string.Empty;
    }
    public class ResetPasswordRequest
    {
        public string SessionToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
