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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email
            };
        }

        public UserResponse Login(LoginUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            User? user = _dbContext.Users.FirstOrDefault(user => user.Email == request.Email);

            if (user is null)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            bool passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!passwordIsValid)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email
            };
        }
    }
}