using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TMAPI_Backend.Data;
using TMAPI_Backend.DTOs.Users;
using TMAPI_Backend.Models;
using TMAPI_Backend.Services;
using Xunit;

namespace TMAPI_UnitTests
{
    public class UserServiceTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private JwtService CreateJwtService()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "THIS_IS_A_TEST_SECRET_KEY_123456789012345",
                    ["Jwt:Issuer"] = "TMAPI_Test",
                    ["Jwt:Audience"] = "TMAPI_Test"
                })
                .Build();

            return new JwtService(configuration);
        }

        private UserService CreateUserService(AppDbContext dbContext)
        {
            return new UserService(dbContext, CreateJwtService());
        }

        [Fact]
        public void Register_ShouldThrowException_WhenEmailIsEmpty()
        {
            var dbContext = CreateDbContext();
            var service = CreateUserService(dbContext);

            var request = new RegisterUserRequest
            {
                Email = "",
                Password = "123456"
            };

            Assert.Throws<ArgumentException>(() => service.Register(request));
        }

        [Fact]
        public void Register_ShouldThrowException_WhenPasswordIsEmpty()
        {
            var dbContext = CreateDbContext();
            var service = CreateUserService(dbContext);

            var request = new RegisterUserRequest
            {
                Email = "test@test.de",
                Password = ""
            };

            Assert.Throws<ArgumentException>(() => service.Register(request));
        }

        [Fact]
        public void Register_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var dbContext = CreateDbContext();

            dbContext.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.de",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            });

            dbContext.SaveChanges();

            var service = CreateUserService(dbContext);

            var request = new RegisterUserRequest
            {
                Email = "test@test.de",
                Password = "abcdef"
            };

            Assert.Throws<InvalidOperationException>(() => service.Register(request));
        }

        [Fact]
        public void Register_ShouldCreateUser_WhenRequestIsValid()
        {
            var dbContext = CreateDbContext();
            var service = CreateUserService(dbContext);

            var request = new RegisterUserRequest
            {
                Email = "test@test.de",
                Password = "123456"
            };

            var result = service.Register(request);

            var savedUser = dbContext.Users.Single();

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("test@test.de", result.Email);
            Assert.Equal("test@test.de", savedUser.Email);
            Assert.True(BCrypt.Net.BCrypt.Verify("123456", savedUser.PasswordHash));
            Assert.NotEqual("123456", savedUser.PasswordHash);
        }

        [Fact]
        public void Login_ShouldThrowException_WhenEmailDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = CreateUserService(dbContext);

            var request = new LoginUserRequest
            {
                Email = "missing@test.de",
                Password = "123456"
            };

            Assert.Throws<InvalidOperationException>(() => service.Login(request));
        }

        [Fact]
        public void Login_ShouldThrowException_WhenPasswordIsWrong()
        {
            var dbContext = CreateDbContext();

            dbContext.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.de",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            });

            dbContext.SaveChanges();

            var service = CreateUserService(dbContext);

            var request = new LoginUserRequest
            {
                Email = "test@test.de",
                Password = "wrong-password"
            };

            Assert.Throws<InvalidOperationException>(() => service.Login(request));
        }

        [Fact]
        public void Login_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            var dbContext = CreateDbContext();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.de",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var service = CreateUserService(dbContext);

            var request = new LoginUserRequest
            {
                Email = "test@test.de",
                Password = "123456"
            };

            var result = service.Login(request);

            Assert.Equal(user.Id, result.Id);
            Assert.Equal("test@test.de", result.Email);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }
    }
}