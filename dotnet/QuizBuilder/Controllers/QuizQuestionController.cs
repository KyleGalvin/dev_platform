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
    public class QuizQuestionController : QuizBaseController
    {
        private readonly ILogger<QuizQuestionController> _logger;
        private readonly QuizQuestionService _quizQuestionService;

        public QuizQuestionController(ILogger<QuizQuestionController> logger, QuizQuestionService quizQuestionService)
        {
            _logger = logger;
            _quizQuestionService = quizQuestionService;

        }

        [HttpPost(Name = "CreateQuizQuestion")]
        public async Task<IActionResult> CreateQuizQuestion([FromBody] QuizQuestion quizQuestion)
        {
            var result = await _quizQuestionService.CreateQuizQuestion(quizQuestion, GetUser());
            if (!result.Success) 
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpPut(Name = "UpdateQuizQuestion")]
        public async Task<IActionResult> UpdateQuizQuestion([FromBody] QuizQuestion quizQuestion)
        {
            var result = await _quizQuestionService.UpdateQuizQuestion(quizQuestion, GetUser());
            if (!result.Success) 
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpDelete(Name = "DeleteQuizQuestion")]
        public async Task<IActionResult> DeleteQuizQuestion(string id)
        {
            var result =await _quizQuestionService.DeleteQuizQuestion(id, GetUser());
            if (!result.Success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpGet(Name = "GetQuizQuestion")]
        public async Task<IActionResult> GetQuizQuestion(string id)
        {
            var result = _quizQuestionService.GetQuizQuestion(id);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }
    }
}
