namespace QubeFin.Auth.Application.Accounts.Model
{
    public class ChangePasswordRequest
    {
        public string Password { get; set; } = null!; 
        public string NewPassword { get; set; } = null!;
    }

    public class ResetPassword
    {
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
