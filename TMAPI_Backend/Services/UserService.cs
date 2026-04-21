using TMAPI_Backend.Data;
using TMAPI_Backend.DTOs.Users;
using TMAPI_Backend.Models;

namespace TMAPI_Backend.Services
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UserResponse Register(RegisterUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            bool userExists = _dbContext.Users.Any(user => user.Email == request.Email);

            if (userExists)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = request.Password
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email
            };
        }
    }
}