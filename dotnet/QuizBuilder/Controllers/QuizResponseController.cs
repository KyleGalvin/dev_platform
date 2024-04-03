using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBuilder.Models;
using QuizBuilder.Services;

namespace QuizBuilder.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class QuizResponseController : QuizBaseController
    {
        private readonly ILogger<QuizResponseController> _logger;
        private readonly QuizResponseService _quizResponseService;
        public QuizResponseController(ILogger<QuizResponseController> logger, QuizResponseService quizResponseService)
        {
            _logger = logger;
            _quizResponseService = quizResponseService;
        }

        [HttpPost(Name = "CreateQuizResponse")]
        public async Task<IActionResult> CreateQuizResponse([FromBody] QuizResponse quizResponse)
        {
            var result = await _quizResponseService.CreateQuizResponse(quizResponse, GetUser().Id);
            if (!result.Success) 
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpDelete(Name = "DeleteQuizResponse")]
        public async Task<IActionResult> DeleteQuizResponse(string quizId, string ownerId)
        {
            var result = await _quizResponseService.DeleteQuizResponse(quizId, ownerId, GetUser().Id);
            if (!result.Success) 
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpGet(Name = "GetQuizResponse")]
        public async Task<IActionResult> GetQuizResponse(string quizId, string ownerId)
        {
            var result = await _quizResponseService.GetQuizResponse(quizId, ownerId, GetUser());
            if (!result.Success) 
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpGet(Name = "GetQuizResponseScore")]
        public async Task<IActionResult> GetQuizResponseScore(string quizId, string ownerId)
        {
            var result = await _quizResponseService.GetQuizResponseScore(quizId, ownerId, GetUser());
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }
    }
}
