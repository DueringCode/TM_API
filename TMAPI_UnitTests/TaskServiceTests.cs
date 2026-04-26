using Microsoft.EntityFrameworkCore;
using TMAPI_Backend.Data;
using TMAPI_Backend.DTOs.Tasks;
using TMAPI_Backend.Models;
using TMAPI_Backend.Services;
using Xunit;

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

        private User CreateUser(AppDbContext dbContext, string email = "test@test.de")
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = "hashed-password"
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            return user;
        }

        private TaskItem CreateTask(AppDbContext dbContext, Guid userId)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Existing Task",
                Description = "Existing Description",
                UserId = userId,
                Status = TMAPI_Backend.Models.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();

            return task;
        }

        [Fact]
        public void Create_ShouldThrowException_WhenTitleIsEmpty()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "",
                Description = "Description",
                UserId = user.Id
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
                Description = "Description",
                UserId = Guid.NewGuid()
            };

            Assert.Throws<InvalidOperationException>(() => service.Create(request));
        }

        [Fact]
        public void Create_ShouldCreateTask_WhenRequestIsValid()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "My Task",
                Description = "Description",
                UserId = user.Id
            };

            var result = service.Create(request);

            var savedTask = dbContext.Tasks.Single();

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("My Task", result.Title);
            Assert.Equal("Description", result.Description);
            Assert.Equal(TMAPI_Backend.Models.TaskStatus.Todo, result.Status);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(result.Id, savedTask.Id);
        }

        [Fact]
        public void GetAllByUser_ShouldReturnOnlyTasksOfGivenUser()
        {
            var dbContext = CreateDbContext();

            var firstUser = CreateUser(dbContext, "first@test.de");
            var secondUser = CreateUser(dbContext, "second@test.de");

            dbContext.Tasks.AddRange(
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "First User Task",
                    UserId = firstUser.Id,
                    Status = TMAPI_Backend.Models.TaskStatus.Todo,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Second User Task",
                    UserId = secondUser.Id,
                    Status = TMAPI_Backend.Models.TaskStatus.Todo,
                    CreatedAt = DateTime.UtcNow
                });

            dbContext.SaveChanges();

            var service = new TaskService(dbContext);

            var result = service.GetAllByUser(firstUser.Id);

            Assert.Single(result);
            Assert.Equal("First User Task", result.Single().Title);
            Assert.Equal(firstUser.Id, result.Single().UserId);
        }

        [Fact]
        public void GetById_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            Assert.Throws<InvalidOperationException>(() => service.GetById(Guid.NewGuid()));
        }

        [Fact]
        public void GetById_ShouldReturnTask_WhenTaskExists()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var task = CreateTask(dbContext, user.Id);

            var service = new TaskService(dbContext);

            var result = service.GetById(task.Id);

            Assert.Equal(task.Id, result.Id);
            Assert.Equal(task.Title, result.Title);
            Assert.Equal(task.Description, result.Description);
            Assert.Equal(task.Status, result.Status);
            Assert.Equal(task.UserId, result.UserId);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenTitleIsEmpty()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var task = CreateTask(dbContext, user.Id);

            var service = new TaskService(dbContext);

            var request = new UpdateTaskRequest
            {
                Title = "",
                Description = "Updated Description",
                Status = TMAPI_Backend.Models.TaskStatus.Done
            };

            Assert.Throws<ArgumentException>(() => service.Update(task.Id, request));
        }

        [Fact]
        public void Update_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = TMAPI_Backend.Models.TaskStatus.Done
            };

            Assert.Throws<InvalidOperationException>(() => service.Update(Guid.NewGuid(), request));
        }

        [Fact]
        public void Update_ShouldUpdateTask_WhenRequestIsValid()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var task = CreateTask(dbContext, user.Id);

            var service = new TaskService(dbContext);

            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = TMAPI_Backend.Models.TaskStatus.Done
            };

            var result = service.Update(task.Id, request);

            var savedTask = dbContext.Tasks.Single();

            Assert.Equal(task.Id, result.Id);
            Assert.Equal("Updated Task", result.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(TMAPI_Backend.Models.TaskStatus.Done, result.Status);

            Assert.Equal("Updated Task", savedTask.Title);
            Assert.Equal("Updated Description", savedTask.Description);
            Assert.Equal(TMAPI_Backend.Models.TaskStatus.Done, savedTask.Status);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            Assert.Throws<InvalidOperationException>(() => service.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void Delete_ShouldRemoveTask_WhenTaskExists()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var task = CreateTask(dbContext, user.Id);

            var service = new TaskService(dbContext);

            service.Delete(task.Id);

            Assert.Empty(dbContext.Tasks);
        }
    }
}