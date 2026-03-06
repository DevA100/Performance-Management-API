using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;
using PerformanceSurvey.Models;
using Microsoft.AspNetCore.Authorization;

namespace PerformanceSurvey.Controllers
{
    [Route("api/PerformanceSurvey/")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("CreateDepartment")]
        public async Task<ActionResult<Department>> CreateDepartment(DepartmentDto request)
        {
            var department = await _departmentService.CreateDepartmentAsync(request);
            if (department == null)
            {
                return StatusCode(500, "Internal server error");
            }
            return Ok(department);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("DepartmentById{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentByIdAsync(int id)
        {
            var departmentDto = await _departmentService.GetDepartmentByIdAsync(id);
            if (departmentDto == null)
            {
                return NotFound();
            }
            return Ok(departmentDto);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("updateDepartmentById/{id}")]
        public async Task<ActionResult<Department>> UpdateDepartment(int id, DepartmentDto departmentDto)
        {
            var department = await _departmentService.UpdateDepartmentAsync(id, departmentDto);
            if (department == null)
            {
                return NotFound("Department does not exist");
            }
            return Ok(department);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAllDepartment")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("disableDepartmentById/{id}")]
        public async Task<ActionResult> DisableDepartmentAsync(int id)
        {
            var result = await _departmentService.DisableDepartmentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok("successful");
        }
    }

}
