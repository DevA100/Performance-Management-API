using PerformanceSurvey.Models.DTOs.ResponseDTOs;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using PerformanceSurvey.Models.RequestDTOs;

namespace PerformanceSurvey.iServices
{
    public interface IAuthLoginService
    {
        Task<CreateAdminUserResponse> CreateAdminUserAsync(CreateAdminUserRequest adminUserDto);
        Task<AuthenticateAdminDto> AuthenticateAsync(string email, string password);
        Task RevokeTokenAsync(string token);
        Task<bool> ChangeAdminPasswordAsync(ChangePasswordDto changePasswordDto);
    }
}
