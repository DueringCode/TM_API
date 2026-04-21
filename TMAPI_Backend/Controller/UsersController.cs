using Microsoft.AspNetCore.Mvc;
using TMAPI_Backend.DTOs.Users;
using TMAPI_Backend.Services;

namespace TMAPI_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public ActionResult<UserResponse> Register(RegisterUserRequest request)
        {
            try
            {
                UserResponse user = _userService.Register(request);
                return CreatedAtAction(nameof(Register), user);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [HttpPost("login")]
        public ActionResult<UserResponse> Login(LoginUserRequest request)
        {
            try
            {
                UserResponse user = _userService.Login(request);
                return Ok(user);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
        }
    }
}