using Microsoft.AspNetCore.Mvc;
using TMAPI_Backend.DTOs.Tasks;
using TMAPI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TMAPI_Backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public ActionResult<List<TaskResponse>> GetAllByUser([FromQuery] Guid userId)
        {
            Guid currentUserId = GetCurrentUserId();

            // nur zu Testzwecken:
            return Ok(new { userIdFromToken = currentUserId });
        }

        [HttpGet("{id}")]
        public ActionResult<TaskResponse> GetById(Guid id)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                TaskResponse task = _taskService.GetById(id, userId);

                return Ok(task);
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (UnauthorizedAccessException exception)
            {
                return Forbid(exception.Message);
            }
        }

        [HttpPost]
        public ActionResult<TaskResponse> Create(CreateTaskRequest request)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                TaskResponse task = _taskService.Create(userId, request);

                return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<TaskResponse> Update(Guid id, UpdateTaskRequest request)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                TaskResponse task = _taskService.Update(id, userId, request);

                return Ok(task);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                _taskService.Delete(id, userId);

                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        private Guid GetCurrentUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("User id is missing.");
            }

            return Guid.Parse(userId);
        }
    }
}