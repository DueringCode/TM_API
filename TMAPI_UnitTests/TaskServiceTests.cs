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
                Description = "Description"
            };

            Assert.Throws<ArgumentException>(() => service.Create(user.Id, request));
        }

        [Fact]
        public void Create_ShouldThrowException_WhenUserDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "My Task",
                Description = "Description"
            };

            Assert.Throws<InvalidOperationException>(() => service.Create(Guid.NewGuid(), request));
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
                Description = "Description"
            };

            var result = service.Create(user.Id, request);

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
            var user = CreateUser(dbContext);
            var service = new TaskService(dbContext);

            Assert.Throws<InvalidOperationException>(() =>
                service.GetById(Guid.NewGuid(), user.Id));
        }

        [Fact]
        public void GetById_ShouldThrowException_WhenTaskBelongsToAnotherUser()
        {
            var dbContext = CreateDbContext();

            var owner = CreateUser(dbContext, "owner@test.de");
            var otherUser = CreateUser(dbContext, "other@test.de");
            var task = CreateTask(dbContext, owner.Id);

            var service = new TaskService(dbContext);

            Assert.Throws<UnauthorizedAccessException>(() =>
                service.GetById(task.Id, otherUser.Id));
        }

        [Fact]
        public void GetById_ShouldReturnTask_WhenTaskExistsAndBelongsToUser()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var task = CreateTask(dbContext, user.Id);

            var service = new TaskService(dbContext);

            var result = service.GetById(task.Id, user.Id);

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

            Assert.Throws<ArgumentException>(() =>
                service.Update(task.Id, user.Id, request));
        }

        [Fact]
        public void Update_ShouldThrowException_WhenTaskDoesNotExist()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var service = new TaskService(dbContext);

            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = TMAPI_Backend.Models.TaskStatus.Done
            };

            Assert.Throws<InvalidOperationException>(() =>
                service.Update(Guid.NewGuid(), user.Id, request));
        }

        [Fact]
        public void Update_ShouldThrowException_WhenTaskBelongsToAnotherUser()
        {
            var dbContext = CreateDbContext();

            var owner = CreateUser(dbContext, "owner@test.de");
            var otherUser = CreateUser(dbContext, "other@test.de");
            var task = CreateTask(dbContext, owner.Id);

            var service = new TaskService(dbContext);

            var request = new UpdateTaskRequest
            {
                Title = "Updated",
                Description = "Updated",
                Status = TMAPI_Backend.Models.TaskStatus.Done
            };

            Assert.Throws<UnauthorizedAccessException>(() =>
                service.Update(task.Id, otherUser.Id, request));
        }

        [Fact]
        public void Update_ShouldUpdateTask_WhenRequestIsValidAndTaskBelongsToUser()
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

            var result = service.Update(task.Id, user.Id, request);

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
            var user = CreateUser(dbContext);
            var service = new TaskService(dbContext);

            Assert.Throws<InvalidOperationException>(() =>
                service.Delete(Guid.NewGuid(), user.Id));
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenTaskBelongsToAnotherUser()
        {
            var dbContext = CreateDbContext();

            var owner = CreateUser(dbContext, "owner@test.de");
            var otherUser = CreateUser(dbContext, "other@test.de");
            var task = CreateTask(dbContext, owner.Id);

            var service = new TaskService(dbContext);

            Assert.Throws<UnauthorizedAccessException>(() =>
                service.Delete(task.Id, otherUser.Id));
        }

        [Fact]
        public void Delete_ShouldRemoveTask_WhenTaskExistsAndBelongsToUser()
        {
            var dbContext = CreateDbContext();
            var user = CreateUser(dbContext);
            var task = CreateTask(dbContext, user.Id);

            var service = new TaskService(dbContext);

            service.Delete(task.Id, user.Id);

            Assert.Empty(dbContext.Tasks);
        }
    }
}