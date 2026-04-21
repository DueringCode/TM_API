using Microsoft.AspNetCore.Mvc;
using TMAPI_Backend.DTOs.Tasks;
using TMAPI_Backend.Services;

namespace TMAPI_Backend.Controllers
{
    [ApiController]
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
            List<TaskResponse> tasks = _taskService.GetAllByUser(userId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public ActionResult<TaskResponse> GetById(Guid id)
        {
            try
            {
                TaskResponse task = _taskService.GetById(id);
                return Ok(task);
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }

        [HttpPost]
        public ActionResult<TaskResponse> Create(CreateTaskRequest request)
        {
            try
            {
                TaskResponse task = _taskService.Create(request);
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
                TaskResponse task = _taskService.Update(id, request);
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
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _taskService.Delete(id);
                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { message = exception.Message });
            }
        }
    }
}