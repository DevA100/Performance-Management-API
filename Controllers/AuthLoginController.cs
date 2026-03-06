using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models.RequestDTOs;

namespace PerformanceSurvey.Controllers
{
    [Route("api/PerformanceSurvey")]
    [ApiController]
    public class AuthLoginController : Controller
    {
        private readonly IAuthLoginService _authLoginService;
        private readonly ILogger<AuthLoginController> _logger;

        public AuthLoginController (IAuthLoginService authLoginService, ILogger<AuthLoginController> logger)
        {
            _authLoginService = authLoginService;
            _logger = logger;
        }

        



        [HttpPost("create-admin-user")]
        public async Task<IActionResult> CreateAdminUser([FromBody] CreateAdminUserRequest createAdminUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createAdminUserDto.Key != "SuperAdminKey#")
            {
                return BadRequest("Invalid Key, Request key from super Admin");
            }
            var adminUser = await _authLoginService.CreateAdminUserAsync(createAdminUserDto);
            if (adminUser == null)
            {
                return BadRequest("Failed to create admin user.");
            }

            return Ok(adminUser);
        }


        


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authLoginService.AuthenticateAsync(loginDto.UserEmail, loginDto.Password);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials" }); 
            }

            return Ok(result); 
        }




        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            token = token.Replace("Bearer ", "");
            await _authLoginService.RevokeTokenAsync(token); 

            return Ok("Token has been revoked");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeAdminPassword")]
        public async Task<IActionResult> ChangeAdminPassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                await _authLoginService.ChangeAdminPasswordAsync(changePasswordDto);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while changing the password." });
            }
        }



    }
}
