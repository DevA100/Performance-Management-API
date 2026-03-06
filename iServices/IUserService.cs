using PerformanceSurvey.Models;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models.DTOs.ResponseDTOs;
using PerformanceSurvey.Models.RequestDTOs.ResponseDTOs;

namespace PerformanceSurvey.iServices
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserRequest userDto);
        Task<UserResponse> GetUserByIdAsync(int id);
        Task<UserResponse> UpdateUserAsync(int id, UserRequest userDto);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<bool> DisableUserAsync(int id);
       
        Task<IEnumerable<UserResponse>> GetUsersByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<UserResponse>> GetUsersByDepartmentIdsAsync(IEnumerable<int> departmentId);
        Task<List<UserResponse>> GetUsersByIdsAsync(List<int> userId);
        Task<string> GetAdminEmailAsync();
        Task<BatchUserResponse> CreateUsersFromExcelAsync(IFormFile file);
    }
}
