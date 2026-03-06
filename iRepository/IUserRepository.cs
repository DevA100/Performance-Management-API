using PerformanceSurvey.Models;
using PerformanceSurvey.Models.DTOs;

namespace PerformanceSurvey.iRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        Task<User> UpdateUserAsync(int id, User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> DisableUserAsync(int id);
        Task<IEnumerable<User>> GetUsersByDepartmentIdAsync(int departmentId);

        Task<IEnumerable<User>> GetUsersByDepartmentIdsAsync(IEnumerable<int> departmentId);


        Task<List<User>> GetUsersByIdsAsync(List<int> userId);

        Task UpdateUserPasswordAsync(User user);
       

        Task<User> GetAdminUserAsync();

    }
}
