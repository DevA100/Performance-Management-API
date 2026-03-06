using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using PerformanceSurvey.Services;
using Microsoft.AspNetCore.Authorization;
using PerformanceSurvey.Models.RequestDTOs;
using PerformanceSurvey.Models.RequestDTOs.ResponseDTOs;

namespace PerformanceSurvey.Controllers
{
    [Route("api/PerformanceSurvey")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _service;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(IQuestionService service, ILogger<QuestionController> logger)
        {
            _service = service;
            _logger = logger;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateMultipleChoiceQuestions")]
        public async Task<ActionResult<MultipleChoiceQuestionResponse>> CreateDepartmentMultipleQuestionAsync(MultipleChoiceQuestionRequest questionDto)
        {
            _logger.LogInformation("Creating a new question");
            var question = await _service.CreateDepartmentMultipleQuestionAsync(questionDto);
            return Ok(question);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("CreateTextQuestions")]
        public async Task<ActionResult<Question>> CreateDepartmentTextQuestionAsync(TextQuestionReqest questionDto)
        {
            _logger.LogInformation("Creating a new question");
            var question = await _service.CreateDepartmentTextQuestionAsync(questionDto);
            return Ok(question);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getQuestionsById/{id}")]
        public async Task<ActionResult<Question>> GetDepartmentQuestion(int id)
        {
            var question = await _service.GetDepartmentQuestionAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAllQuestions")]
        public async Task<ActionResult<IEnumerable<Question>>> GetAllDepartmentQuestions()
        {
            var questions = await _service.GetAllDepartmentQuestionsAsync();
            return Ok(questions);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("updateMultipleChoiceQuestions/{id}")]
        public async Task<ActionResult<Question>> UpdateDepartmentMultipleChoiceQuestion(int id, MultipleChoiceQuestionRequest questionDto)
        {
            var updatedQuestion = await _service.UpdateDepartmentMultipleChoiceQuestionAsync(id, questionDto);
            if (updatedQuestion == null)
            {
                return NotFound();
            }
            return Ok(updatedQuestion);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("QuestionOptionById{optionId}")]
        public async Task<IActionResult> GetOptionById(int optionId)
        {
            var option = await _service.GetOptionByIdAsync(optionId);
            if (option == null)
            {
                return NotFound($"Option with ID {optionId} not found.");
            }
            return Ok(option);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllQuestionOption")]
        public async Task<IActionResult> GetAllOptions()
        {
            var options = await _service.GetAllOptionsAsync();
            return Ok(options);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("updateTextQuestion/{id}")]
        public async Task<ActionResult<TextQuestionResponse>> UpdateDepartmentTextQuestion(int id, TextQuestionReqest questionDto)
        {
            var updatedQuestion = await _service.UpdateDepartmentTextQuestionAsync(id, questionDto);
            if (updatedQuestion == null)
            {
                return NotFound();
            }
            return Ok(updatedQuestion);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteQuestionById/{id}")]
        public async Task<ActionResult<Question>> DeleteDepartmentQuestion(int id)
        {
            var question = await _service.DeleteDepartmentQuestionAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok("Deleted Successfully");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("QuestionsByDepartmentId/{departmentId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetDepartmentQuestionsByDepartmentId(int departmentId)
        {
            _logger.LogInformation("Fetching questions for department ID {DepartmentId}", departmentId);
            var questions = await _service.GetDepartmentQuestionsByDepartmentIdAsync(departmentId);
            if (questions == null || !questions.Any())
            {
                return NotFound();
            }
            return Ok(questions);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("QuestionsByDepartmentIds")]
        public async Task<IActionResult> GetQuestionsByDepartmentIds([FromQuery] List<int> departmentIds)
        {
            if (departmentIds == null || departmentIds.Count == 0)
            {
                return BadRequest("Department IDs are required.");
            }

            var questions = await _service.GetDepartmentQuestionsByDepartmentIdsAsync(departmentIds);

            if (questions == null)
            {
                return NotFound("No questions found for the specified departments.");
            }

            return Ok(questions);
        }
    }
}


