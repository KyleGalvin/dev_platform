using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizBuilder.Models;
using QuizBuilder.Services;
using QuizBuilder.Util;

namespace QuizBuilder.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class QuizController : QuizBaseController
    {
        private readonly ILogger<QuizController> _logger;
        private readonly QuizService _quizService;
        public QuizController(ILogger<QuizController> logger, QuizService quizService)
        {
            _logger = logger;
            _quizService = quizService;
        }

        [HttpPost(Name = "CreateQuiz")]
        public async Task<IActionResult> CreateQuiz([FromBody] Quiz quiz)
        {
            var result = await _quizService.CreateQuiz(quiz, GetUser().Id);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpPut(Name = "UpdateQuiz")]
        public async Task<IActionResult> UpdateQuiz([FromBody] Quiz quiz)
        {
            var result = await _quizService.UpdateQuiz(quiz, GetUser());
            if (!result.Success) 
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpDelete(Name = "DeleteQuiz")]
        public async Task<IActionResult> DeleteQuiz(string id)
        {
            var result = await _quizService.DeleteQuiz(id, GetUser());
            if (!result.Success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpGet(Name = "GetQuiz")]
        public async Task<IActionResult> GetQuiz(string id)
        {
            var result = _quizService.GetQuiz(id);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpGet(Name = "GetQuizzes")]
        public async Task<IActionResult> GetQuizzes(int top, int skip)
        {
           var result = _quizService.GetQuizzes(top, skip);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpGet(Name = "GetMyAnsweredQuizzes")]
        public async Task<IActionResult> GetMyAnsweredQuizzes(int top, int skip)
        {
            var result = _quizService.GetMyAnsweredQuizzes(top, skip, GetUser());
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpPut(Name = "PublishQuiz")]
        public async Task<IActionResult> PublishQuiz(string id)
        {
            var result = await _quizService.PublishQuiz(id);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

    }
}
