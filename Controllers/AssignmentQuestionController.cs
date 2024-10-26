using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;

namespace PerformanceSurvey.Controllers
{
    [Route("api/PerformanceSurvey")]
    [ApiController]
    public class AssignmentQuestionController : ControllerBase
    {
        private readonly IAssignmentQuestionService _assignmentQuestionService;

        public AssignmentQuestionController(IAssignmentQuestionService assignmentQuestionService)
        {
            _assignmentQuestionService = assignmentQuestionService;
        }


        //[Authorize(Roles = "Admin")]
        // POST: api/AssignmentQuestion/AssignMultipleQuestionsToMultipleUsers
        [HttpPost("AssignQuestionsToSingleUser")]
        public async Task<IActionResult> AssignQuestionsToSingleUser([FromBody] AssignmentQuestionSingleUserDto assignmentQuestionSingleUserDto)
        {
            if (assignmentQuestionSingleUserDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                await _assignmentQuestionService.AssignQuestionsToSingleUsersAsync(assignmentQuestionSingleUserDto);
                return Ok("Questions assigned successfully to the single user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        // POST: api/AssignmentQuestion/AssignToMultipleUsers
        [HttpPost("AssignQuestionsToMultipleUsers")]
        public async Task<IActionResult> AssignQuestionsToMultipleUsers([FromBody] AssignmentQuestionMultipleDto assignmentQuestionMultipleDto)
        {
            if (assignmentQuestionMultipleDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                await _assignmentQuestionService.AssignQuestionsToMultipleUsersAsync(assignmentQuestionMultipleDto);
                return Ok("Assignments created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        // POST: api/AssignmentQuestion/AssignToDepartment
        [HttpPost("AssignQuestionsToDepartment")]
        public async Task<IActionResult> AssignQuestionsToDepartment([FromBody] AssignQuestionsToDepartmentDto assignmentQuestionMultipleDto)
        {
            if (assignmentQuestionMultipleDto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                await _assignmentQuestionService.AssignQuestionsToDepartmentAsync(assignmentQuestionMultipleDto);
                return Ok("Questions assigned successfully to the department.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("AssignDifferentQuestionsToDepartment")]

        public async Task<IActionResult> AssignDifferentQuestionsToDepartment([FromBody] AssignDiffQuestionsToDepartmentDto dto)
        {
            if (dto == null || !dto.SourceDepartmentId.Any() || dto.TargetDepartmentId == 0)
            {
                return BadRequest("Invalid input data.");
            }

            try
            {
                await _assignmentQuestionService.AssignDiffQuestionsToDepartmentAsync(dto);
                return Ok("Questions have been successfully assigned to the department.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return StatusCode(500, "An error occurred while assigning questions.");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("AssignDiffQuestionsToDiffDepartment")]
        public async Task<IActionResult> AssignDiffQuestionsToDiffDepartment([FromBody] AssignDiffQuestionsToDiffDepartmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _assignmentQuestionService.AssignDiffQuestionsToDiffDepartmentAsync(dto);
                return Ok("Questions assigned successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is set up)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("AssignedQuestionByUserID/{userId}")]
        public async Task<IActionResult> GetAssignmentQuestionsByUserId(int userId)
        {
            try
            {
                var questions = await _assignmentQuestionService.GetAssignmentQuestionsByUserIdAsync(userId);

                // Check if questions were found
                if (questions == null || !questions.Any())
                {
                    return NotFound($"No assignment questions found for user with ID {userId}");
                }

                return Ok(questions);
            }
            catch (Exception ex)
            {
                // Log the exception details (e.g., using a logging framework like Serilog)
                return StatusCode(500, $"An error occurred while retrieving assignment questions: {ex.Message}");
            }
        }

        [HttpGet("AssignedQuestionByUserIDs/{userIds}")]
        public async Task<IActionResult> GetAssignmentQuestionsByUserIds([FromQuery] List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return BadRequest("User IDs cannot be empty");
            }

            try
            {
                var questions = await _assignmentQuestionService.GetAssignmentQuestionsByUserIdsAsync(userIds);

                // Check if questions were found
                if (questions == null || !questions.Any())
                {
                    return NotFound("No assignment questions found for the provided user IDs");
                }

                return Ok(questions);
            }
            catch (Exception ex)
            {
                // Log the exception details (e.g., using a logging framework like Serilog)
                return StatusCode(500, $"An error occurred while retrieving assignment questions: {ex.Message}");
            }
        }

        [HttpDelete("DeleteAnsweredQuestions/{userId}")]
        public async Task<IActionResult> DeleteAnsweredQuestions(int userId)
        {
            try
            {
                await _assignmentQuestionService.DeleteAnsweredAssignmentQuestionsByUserIdAsync(userId);
                return Ok(200); // Return 204 No Content if deletion is successful
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [HttpGet("users-with-pending-assignments")]
        public async Task<IActionResult> GetUsersWithPendingAssignments()
        {
            var users = await _assignmentQuestionService.GetUsersWithPendingAssignmentsAsync();

            if (!users.Any())
            {
                return NotFound("No users with pending assignments found.");
            }

            return Ok(users);
        }


    }

}
