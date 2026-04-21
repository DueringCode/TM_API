using Microsoft.EntityFrameworkCore;
using TMAPI_Backend.Data;
using TMAPI_Backend.DTOs.Tasks;
using TMAPI_Backend.Models;
using TMAPI_Backend.Services;
using TaskStatus = TMAPI_Backend.Models.TaskStatus;

namespace TMAPI_UnitTests
{
    public class TaskServiceTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Create_ShouldThrowException_WhenTitleIsEmpty()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "",
                Description = "Test description",
                UserId = Guid.NewGuid()
            };

            Assert.Throws<ArgumentException>(() => service.Create(request));
        }

        [Fact]
        public void Create_ShouldThrowException_WhenUserDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "My Task",
                Description = "Test description",
                UserId = Guid.NewGuid()
            };

            Assert.Throws<InvalidOperationException>(() => service.Create(request));
        }

        [Fact]
        public void Create_ShouldReturnTaskResponse_WhenRequestIsValid()
        {
            var dbContext = CreateDbContext();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@test.de",
                PasswordHash = "hashed-password"
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "My Task",
                Description = "Test description",
                UserId = user.Id
            };

            var result = service.Create(request);

            Assert.NotNull(result);
            Assert.Equal("My Task", result.Title);
            Assert.Equal("Test description", result.Description);
            Assert.Equal(TaskStatus.Todo, result.Status);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public void GetById_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            Assert.Throws<InvalidOperationException>(() => service.GetById(Guid.NewGuid()));
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            Assert.Throws<InvalidOperationException>(() => service.Delete(Guid.NewGuid()));
        }
    }
}