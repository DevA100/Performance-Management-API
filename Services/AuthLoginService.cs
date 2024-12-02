using PerformanceSurvey.Models.DTOs.ResponseDTOs;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Utilities;
using PerformanceSurvey.iRepository;
using PerformanceSurvey.Repository;
using System.Security.Cryptography;
using System.Text;
using PerformanceSurvey.Models.RequestDTOs;

namespace PerformanceSurvey.Services
{
    public class AuthLoginService : IAuthLoginService
    {
        private readonly IAuthLoginRepository _authLoginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly JwtTokenUtil _jwtTokenUtil;

        public AuthLoginService(IAuthLoginRepository authLoginRepository, JwtTokenUtil jwtTokenUtil, IUserRepository userRepository, IEmailService emailService)
        {
            _authLoginRepository = authLoginRepository;
            _emailService = emailService;
            _userRepository = userRepository;
            _jwtTokenUtil = jwtTokenUtil;
        }

        public async Task<CreateAdminUserResponse> CreateAdminUserAsync(CreateAdminUserRequest adminUserDto)
        {
            // Hash the provided password
            var hashedPassword = HashPassword(adminUserDto.Password);

            // Create the admin user entity
            var adminUser = new User
            {
                Name = adminUserDto.Name,
                UserEmail = adminUserDto.UserEmail,
                Password = hashedPassword,
                UserType = UserType.AdminUser,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDisabled = false // Set initial status
            };

            // Save admin user to the database
            await _authLoginRepository.CreateAdminUserAsync(adminUser);

            // Send email to the newly created admin
            var emailSubject = "Admin Account Created";
            var emailBody = $"Hello {adminUser.Name},\n\n" +
                  "Your admin account has been created successfully. Here are your login details:\n\n" +
                  $"Email: {adminUser.UserEmail}\n" +
                  $"Password: {adminUserDto.Password}\n\n" +
                  $"You can log in using the URL: https://performanceportal.jubileelifeng.com/performancesurvey/";


            // Use the email service to send the email
            await _emailService.SendEmailAsync(adminUser.UserEmail, emailSubject, emailBody);

            // Map to UserDto and return
            return new CreateAdminUserResponse
            {
                Name = adminUser.Name,
                UserEmail = adminUser.UserEmail,
                Password = adminUserDto.Password,
            };
        }



        public async Task<AuthenticateAdminDto> AuthenticateAsync(string email, string password)
        {
            // Fetch the user by email
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null; // Return null if the user does not exist
            }

            // Verify the password
            var hashedInputPassword = HashPassword(password);
            if (user.Password != hashedInputPassword)
            {
                return null; // Return null if the password does not match
            }

            // Determine the role based on the UserType property
            string role;
            if (user.UserType == UserType.AdminUser)
            {
                role = "Admin";
            }
            else if (user.UserType == UserType.User)
            {
                role = "User";
            }
            else
            {
                return null; // Return null if the user type is not recognized
            }

            // Generate a token with the user's role
            var token = _jwtTokenUtil.GenerateToken(email, role);

            // Return the response DTO with the token, expiration, and role information
            return new AuthenticateAdminDto
            {
                Token = token,
                IssuedOn = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(60),
                Role = role,
                UserId = user.UserId
            };
        }









        //public async Task<AuthenticateAdminDto> AuthenticateAdminUserAsync(string email, string password)
        //{
        //    // Fetch user by email
        //    var user = await _userRepository.GetUserByEmailAsync(email);
        //    if (user == null || user.UserType != UserType.AdminUser)
        //    {
        //        return null; // Return null if user is not found or not an admin
        //    }

        //    // Verify the password
        //    var hashedInputPassword = HashPassword(password);
        //    if (user.Password != hashedInputPassword)
        //    {
        //        return null; // Return null if password does not match
        //    }

        //    var token = _jwtTokenUtil.GenerateToken(email, "Admin");
        //    // Return user DTO
        //    return new AuthenticateAdminDto
        //    {
        //        Token = token,
        //        IssuedOn = DateTime.UtcNow,
        //        ExpiredAt = DateTime.UtcNow .AddMinutes(60),
        //    };
        //}

        //public async Task<AuthenticateAdminDto> AuthenticateUserAsync(string email, string password)
        //{
        //    // Step 1: Retrieve the user by email
        //    var user = await _userRepository.GetUserByEmailAsync(email);
        //    if (user == null || user.UserType == UserType.AdminUser)
        //    {
        //        return null; // User not found
        //    }

        //    // Step 2: Verify the password by hashing the input and comparing it with the stored hash
        //    var hashedInputPassword = HashPassword(password);
        //    if (user.Password != hashedInputPassword)
        //    {
        //        return null; // Password does not match
        //    }

        //    var token = _jwtTokenUtil.GenerateToken(email, "Users");

        //    // Step 3: Return the user if authentication is successful
        //    return new AuthenticateAdminDto
        //    {
        //        Token = token,
        //        IssuedOn = DateTime.UtcNow,
        //        ExpiredAt = DateTime.UtcNow.AddMinutes(60),
        //    };
        //}

        // HashPassword method should be the same as the one used during password generation
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }



        public async Task RevokeTokenAsync(string token)
        {
            await _authLoginRepository.RevokeTokenAsync(token);
        }

        public async Task<bool> ChangeAdminPasswordAsync(ChangePasswordDto changePasswordDto)
        {
            // Fetch user by email
            var user = await _userRepository.GetUserByEmailAsync(changePasswordDto.Email);

            // Check if user exists and is an admin
            if (user == null || user.UserType != UserType.AdminUser)
            {
                throw new ArgumentException("Admin user not found.");
            }

            // Verify the current password
            var hashedCurrentPassword = HashPassword(changePasswordDto.CurrentPassword);
            if (user.Password != hashedCurrentPassword)
            {
                throw new ArgumentException("Current password is incorrect.");
            }

            // Hash the new password
            var hashedNewPassword = HashPassword(changePasswordDto.NewPassword);

            // Update the user's password
            user.Password = hashedNewPassword;
            await _userRepository.UpdateUserPasswordAsync(user);

            // Optionally, send a notification email about the password change (if needed)
            var emailBody = $"Hello {user.Name},<br>Your password has been successfully changed.";
            await _emailService.SendEmailAsync(user.UserEmail, "Password Changed", emailBody);

            return true;
        }

    }

}

