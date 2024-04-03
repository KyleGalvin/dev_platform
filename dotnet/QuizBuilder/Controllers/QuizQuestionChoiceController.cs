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
    public class QuizQuestionChoiceController : QuizBaseController
    {
        private readonly ILogger<QuizController> _logger;
        private readonly QuizQuestionChoiceService _quizQuestionChoiceService;
        public QuizQuestionChoiceController(ILogger<QuizController> logger, QuizQuestionChoiceService quizQuestionChoiceService)
        {
            _logger = logger;
            _quizQuestionChoiceService = quizQuestionChoiceService;
        }

        [HttpPost(Name = "CreateQuizQuestionChoice")]
        public async Task<IActionResult> CreateQuizQuestionChoice([FromBody] QuizQuestionChoice quizQuestionChoice)
        {
            var result = await _quizQuestionChoiceService.CreateQuizQuestionChoice(quizQuestionChoice, GetUser());
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpPut(Name = "UpdateQuizQuestionChoice")]
        public async Task<IActionResult> UpdateQuizQuestionChoice([FromBody] QuizQuestionChoice quizQuestionChoice)
        {
            var result = await _quizQuestionChoiceService.UpdateQuizQuestionChoice(quizQuestionChoice, GetUser());
            if (!result.Success) 
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        [HttpDelete(Name = "DeleteQuizQuestionChoice")]
        public async Task<IActionResult> DeleteQuizQuestionChoice(string id)
        {
            var result = await _quizQuestionChoiceService.DeleteQuizQuestionChoice(id, GetUser());
            if (!result.Success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpGet(Name = "GetQuizQuestionChoice")]
        public async Task<IActionResult> GetQuizQuestionChoice(string id)
        {
            var result = _quizQuestionChoiceService.GetQuizQuestionChoice(id);
            if (!result.Success)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }
    }
}
