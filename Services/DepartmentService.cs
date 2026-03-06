using PerformanceSurvey.iRepository;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using PerformanceSurvey.Models.RequestDTOs.ResponseDTOs;

namespace PerformanceSurvey.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository repository, ILogger<DepartmentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto)
        {
            _logger.LogInformation("Creating a new Department with Name {DepartmentName}", departmentDto.DepartmentName);

            var department = new Department
            {
                DepartmentName = departmentDto.DepartmentName,
                CreatedAt = DateTime.UtcNow 
            };

            var createdDepartment = await _repository.CreateDepartmentAsync(department);

            var resultDto = new DepartmentDto
            {
                DepartmentName = createdDepartment.DepartmentName,
            };

            return resultDto;
        }


        public async Task<DepartmentDto> GetDepartmentByIdAsync(int id)
        {
            var department = await _repository.GetDepartmentByIdAsync(id);
            if (department == null || department.IsDisabled)
                return null;

            return new DepartmentDto
            {
                DepartmentName = department.DepartmentName
            };
        }


        public async Task<IEnumerable<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            var departments = await _repository.GetAllDepartmentsAsync();
            return departments.Select(d => new DepartmentResponseDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName
            });
        }

        public async Task<DepartmentDto> UpdateDepartmentAsync(int id, DepartmentDto departmentDto)
        {
            var department = await _repository.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return null; 
            }

            department.DepartmentName = departmentDto.DepartmentName;
            department.UpdatedAt = DateTime.UtcNow;

            var updatedDepartment = await _repository.UpdateDepartmentAsync(department);

            var updatedDepartmentDto = new DepartmentDto
            {
                DepartmentName = updatedDepartment.DepartmentName,
            };

            return updatedDepartmentDto; 
        }

        public async Task<bool> DisableDepartmentAsync(int id)
        {
            await _repository.DisableDepartmentAsync(id);
            return true;
        }
    }

}
