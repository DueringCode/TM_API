namespace TMAPI_Backend.DTOs.Users
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}