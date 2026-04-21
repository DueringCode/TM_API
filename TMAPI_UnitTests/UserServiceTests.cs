using Microsoft.EntityFrameworkCore;
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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Register_ShouldThrowException_WhenEmailIsEmpty()
        {
            var dbContext = CreateDbContext();
            var service = new UserService(dbContext);

            var request = new RegisterUserRequest
            {
                Email = "",
                Password = "123456"
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
                PasswordHash = "123456"
            });

            dbContext.SaveChanges();

            var service = new UserService(dbContext);

            var request = new RegisterUserRequest
            {
                Email = "test@test.de",
                Password = "abcdef"
            };

            Assert.Throws<InvalidOperationException>(() => service.Register(request));
        }
    }
}