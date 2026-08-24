namespace QubeFin.App.Api.Requests
{
    public class UserCreateRequest
    {
        public Guid EmployeeId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
