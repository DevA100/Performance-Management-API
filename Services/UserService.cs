using PerformanceSurvey.iRepository;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Repository;
using System.Text;
using ClosedXML.Excel;


using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PerformanceSurvey.Utilities;
using PerformanceSurvey.Models.DTOs.ResponseDTOs;
using PerformanceSurvey.Models.RequestDTOs.ResponseDTOs;

namespace PerformanceSurvey.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _repository;

        public UserService(IUserRepository userRepository,JwtTokenUtil jwtTokenUtil, IDepartmentRepository repository)

        {
            _userRepository = userRepository;
            _repository = repository; ;
        }

        public async Task<UserResponse> CreateUserAsync(UserRequest userDto)
        {
            if (!IsValidEmail(userDto.UserEmail))
            {
                throw new ArgumentException("Invalid email format.");
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.UserEmail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already in use.");
            }


            var user = new User
            {
                Name = userDto.Name,
                UserEmail = userDto.UserEmail,
                DepartmentId = userDto.DepartmentId,
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            return new UserResponse
            {
                Name = createdUser.Name,
                UserEmail = createdUser.UserEmail,
            };
        }

        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null || user.IsDisabled) return null; 

            return new UserResponse
            {
                Name = user.Name,
                UserEmail = user.UserEmail,
            };
        }

        public async Task<List<UserResponse>> GetUsersByIdsAsync(List<int> userIds)
        {
            var users = await _userRepository.GetUsersByIdsAsync(userIds);

            var userDtos = users.Select(user => new UserResponse
            {
                Name = user.Name,
                UserEmail = user.UserEmail,
            }).ToList();

            return userDtos;
        }


        public async Task<UserResponse> UpdateUserAsync(int id, UserRequest userDto)
        {
            var user = new User
            {
                UserId = id, 
                Name = userDto.Name,
                UserEmail = userDto.UserEmail,
                DepartmentId = userDto.DepartmentId,
            };

            var updatedUser = await _userRepository.UpdateUserAsync(id, user);
            if (updatedUser == null) return null;

            return new UserResponse
            {
                Name = updatedUser.Name,
                UserEmail = updatedUser.UserEmail,
            };
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync(); 
            return users.Select(user => new UserResponse
            {
                UserId =user.UserId,
                Name = user.Name,
                UserEmail = user.UserEmail,
            });
        }

        public async Task<bool> DisableUserAsync(int id)
        {
            return await _userRepository.DisableUserAsync(id);
        }

        public async Task<IEnumerable<UserResponse>> GetUsersByDepartmentIdAsync(int departmentId)
        {
            var users = await _userRepository.GetUsersByDepartmentIdAsync(departmentId);
            return users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                UserEmail = user.UserEmail,
            });
        }
        public async Task<IEnumerable<UserResponse>> GetUsersByDepartmentIdsAsync(IEnumerable<int> departmentId)
        {
            if (departmentId == null || !departmentId.Any())
            {
                return Enumerable.Empty<UserResponse>();
            }

            var users = await _userRepository.GetUsersByDepartmentIdsAsync(departmentId);

            var userDtos = users.Select(u => new UserResponse
            {
                Name = u.Name,
                UserEmail = u.UserEmail,
            }).ToList();

            return userDtos;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public async Task<string> GetAdminEmailAsync()
        {
            var adminUser = await _userRepository.GetAdminUserAsync();
            return adminUser?.UserEmail;
        }





        public async Task<BatchUserResponse> CreateUsersFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException("File is empty or invalid");
            }
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("File must be an Excel file (.xlsx)");
            }

            var response = new BatchUserResponse();
            var validUsers = new List<UserRequest>();
            var errors = new List<string>();

            var departments = await _repository.GetAllDepartmentsAsync();
            var departmentDictionary = departments.ToDictionary(d => d.DepartmentName.ToLower(), d => d.DepartmentId);

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1); 

                    foreach (var row in rows)
                    {
                        try
                        {
                            var name = row.Cell(1).Value.ToString().Trim();
                            var email = row.Cell(2).Value.ToString().Trim();
                            var departmentName = row.Cell(3).Value.ToString().Trim().ToLower();

                            if (!departmentDictionary.TryGetValue(departmentName, out int departmentId))
                            {
                                errors.Add($"Invalid department name '{departmentName}' in row {row.RowNumber()}");
                                continue;
                            }

                            var userRequest = new UserRequest
                            {
                                Name = name,
                                UserEmail = email,
                                DepartmentId = departmentId
                            };

                            if (string.IsNullOrEmpty(userRequest.Name) ||
                                string.IsNullOrEmpty(userRequest.UserEmail))
                            {
                                errors.Add($"Invalid data in row {row.RowNumber()}");
                                continue;
                            }

                            validUsers.Add(userRequest);
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Error processing row {row.RowNumber()}: {ex.Message}");
                        }
                    }

                    foreach (var userRequest in validUsers)
                    {
                        try
                        {
                            await CreateUserAsync(userRequest);
                            response.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Failed to create user {userRequest.UserEmail}: {ex.Message}");
                        }
                        response.TotalProcessed++;
                    }
                }
            }

            response.Errors = errors;
            response.Message = $"Processed {response.TotalProcessed} users. Successfully created {response.SuccessCount} users.";

            return response;
        }



    }

}
