using TMAPI_Backend.Data;
using TMAPI_Backend.DTOs.Tasks;
using TMAPI_Backend.Models;

namespace TMAPI_Backend.Services
{
    public class TaskService
    {
        private readonly AppDbContext _dbContext;

        public TaskService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TaskResponse> GetAllByUser(Guid userId)
        {
            List<TaskResponse> tasks = _dbContext.Tasks
                .Where(task => task.UserId == userId)
                .Select(task => new TaskResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt,
                    UserId = task.UserId
                })
                .ToList();

            return tasks;
        }

        public TaskResponse GetById(Guid id)
        {
            TaskItem? task = _dbContext.Tasks.FirstOrDefault(task => task.Id == id);

            if (task is null)
            {
                throw new InvalidOperationException("Task not found.");
            }

            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UserId = task.UserId
            };
        }

        public TaskResponse Create(CreateTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title is required.");
            }

            bool userExists = _dbContext.Users.Any(user => user.Id == request.UserId);

            if (!userExists)
            {
                throw new InvalidOperationException("User not found.");
            }

            TaskItem task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = Models.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId
            };

            _dbContext.Tasks.Add(task);
            _dbContext.SaveChanges();

            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UserId = task.UserId
            };
        }

        public TaskResponse Update(Guid id, UpdateTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title is required.");
            }

            TaskItem? task = _dbContext.Tasks.FirstOrDefault(task => task.Id == id);

            if (task is null)
            {
                throw new InvalidOperationException("Task not found.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;

            _dbContext.SaveChanges();

            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UserId = task.UserId
            };
        }

        public void Delete(Guid id)
        {
            TaskItem? task = _dbContext.Tasks.FirstOrDefault(task => task.Id == id);

            if (task is null)
            {
                throw new InvalidOperationException("Task not found.");
            }

            _dbContext.Tasks.Remove(task);
            _dbContext.SaveChanges();
        }
    }
}