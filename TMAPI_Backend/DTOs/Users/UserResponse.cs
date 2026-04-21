namespace TMAPI_Backend.DTOs.Users
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}