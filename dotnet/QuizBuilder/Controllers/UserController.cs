using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBuilder.Services;

namespace QuizBuilder.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : QuizBaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(ILogger<UserController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost(Name = "Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _userService.Login(email, password);
            if (!result.Success) 
            {
                return Forbid();
            }
            return Ok(result.Value);
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser(string email, string password)
        {
            var result = await _userService.CreateUser(email, password);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpDelete(Name = "DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser (string userId)
        {
            var result = await _userService.DeleteUser(userId);
            if (!result.Success)
            {
                return NotFound(result.Errors);
            }
           
            return NoContent();
        }
    }
}
