using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using PerformanceSurvey.iServices;
using Microsoft.AspNetCore.Authorization;
using PerformanceSurvey.Services;
using PerformanceSurvey.Models.DTOs.ResponseDTOs;

namespace PerformanceSurvey.Controllers
{
    [Route("api/PerformanceSurvey")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        
        public UserController(IUserService userService, ILogger<UserController> logger )
        {
            _userService = userService;
            _logger = logger;
            
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("CreateUsers")]
        public async Task<ActionResult<UserRequest>> CreateUserAsync([FromBody] UserRequest userDto)
        {
            try
            {
                _logger.LogInformation("Creating a new user with email {UserEmail}.", userDto.UserEmail);
                var createdUser = await _userService.CreateUserAsync(userDto);
                return Ok(createdUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error occurred while creating user.");
                return BadRequest(new { message = "Check your email address: " + ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict error occurred while creating user.");
                return Conflict(new { message = "A conflict occurred: " + ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                return StatusCode(500, "Internal server error");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("UsersById/{id}")]
        public async Task<ActionResult<UserResponse>> GetUserByIdAsync(int id)
        {
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }
            return Ok(userDto);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("users/by-ids")]
        public async Task<IActionResult> GetUsersByIds([FromQuery] List<int> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return BadRequest("No user IDs provided.");
            }

            var users = await _userService.GetUsersByIdsAsync(userIds);

            if (users == null || !users.Any())
            {
                return NotFound("No users found for the provided IDs.");
            }

            return Ok(users);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("UsersByDepartmentIds")]
        public async Task<IActionResult> GetUsersByDepartmentIds([FromQuery] IEnumerable<int> departmentIds)
        {
            if (departmentIds == null || !departmentIds.Any())
            {
                return BadRequest("Department IDs are required.");
            }

            var users = await _userService.GetUsersByDepartmentIdsAsync(departmentIds);

            if (users == null || !users.Any())
            {
                return NotFound("No users found for the provided department IDs.");
            }

            return Ok(users);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("updateUserById/{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserRequest userDto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, userDto);
                if (updatedUser == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<IEnumerable<UserRequest>>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("disableUsersById/{id}")]
        public async Task<IActionResult> DisableUser(int id)
        {
            var result = await _userService.DisableUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok("Successful");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("userByDepartmentId/{departmentId}")]
        public async Task<ActionResult<IEnumerable<UserRequest>>> GetUsersByDepartmentIdAsync(int departmentId)
        {
            var users = await _userService.GetUsersByDepartmentIdAsync(departmentId);
            if (users == null || !users.Any())
            {
                return NotFound();
            }
            return Ok(users);
        }


        [HttpGet("admin-email")]
        public async Task<IActionResult> GetAdminEmail()
        {
            var email = await _userService.GetAdminEmailAsync();
            if (string.IsNullOrEmpty(email))
            {
                return NotFound("Admin email not found.");
            }
            return Ok(email);
        }
    }


}
