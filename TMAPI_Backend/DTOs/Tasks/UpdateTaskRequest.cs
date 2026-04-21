using TMAPI_Backend.Models;

namespace TMAPI_Backend.DTOs.Tasks
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Models.TaskStatus Status { get; set; }
    }
}