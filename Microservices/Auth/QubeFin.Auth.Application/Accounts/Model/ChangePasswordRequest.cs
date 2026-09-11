namespace QubeFin.Auth.Application.Accounts.Model
{
    public class ChangePasswordRequest
    {
        public string Password { get; set; } = null!; 
        public string NewPassword { get; set; } = null!;
    }
}
