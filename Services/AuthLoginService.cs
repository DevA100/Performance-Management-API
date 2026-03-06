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
            var hashedPassword = HashPassword(adminUserDto.Password);

            var adminUser = new User
            {
                Name = adminUserDto.Name,
                UserEmail = adminUserDto.UserEmail,
                Password = hashedPassword,
                UserType = UserType.AdminUser,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDisabled = false 
            };

            await _authLoginRepository.CreateAdminUserAsync(adminUser);

            var emailSubject = "Admin Account Created";
            var emailBody = $"Hello {adminUser.Name},\n\n" +
                  "Your admin account has been created successfully. Here are your login details:\n\n" +
                  $"Email: {adminUser.UserEmail}\n" +
                  $"Password: {adminUserDto.Password}\n\n" +
                  $"You can log in using the URL: https://example.com/";


            await _emailService.SendEmailAsync(adminUser.UserEmail, emailSubject, emailBody);

            return new CreateAdminUserResponse
            {
                Name = adminUser.Name,
                UserEmail = adminUser.UserEmail,
                Password = adminUserDto.Password,
            };
        }



        public async Task<AuthenticateAdminDto> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null; 
            }

            var hashedInputPassword = HashPassword(password);
            if (user.Password != hashedInputPassword)
            {
                return null; 
            }

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
                return null; 
            }

            var token = _jwtTokenUtil.GenerateToken(email, role);

            
            return new AuthenticateAdminDto
            {
                Token = token,
                IssuedOn = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(60),
                Role = role,
                UserId = user.UserId
            };
        }









        
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
            var user = await _userRepository.GetUserByEmailAsync(changePasswordDto.Email);

            if (user == null || user.UserType != UserType.AdminUser)
            {
                throw new ArgumentException("Admin user not found.");
            }

            var hashedCurrentPassword = HashPassword(changePasswordDto.CurrentPassword);
            if (user.Password != hashedCurrentPassword)
            {
                throw new ArgumentException("Current password is incorrect.");
            }

            var hashedNewPassword = HashPassword(changePasswordDto.NewPassword);

            user.Password = hashedNewPassword;
            await _userRepository.UpdateUserPasswordAsync(user);

            var emailBody = $"Hello {user.Name},<br>Your password has been successfully changed.";
            await _emailService.SendEmailAsync(user.UserEmail, "Password Changed", emailBody);

            return true;
        }

    }

}

