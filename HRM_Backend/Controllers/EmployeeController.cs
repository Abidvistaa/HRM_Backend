using HRM_Backend.Model;
using HRM_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var list = await _employeeService.GetAllAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("GetEmployeeById/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            return Ok(employee);
        }

        [Authorize(Roles = "HR")]
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            await _employeeService.AddAsync(employee);

            return Ok(new
            {
                Message = "Employee created successfully."
            });
        }

        [Authorize(Roles = "HR")]
        [HttpPut("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id,Employee employee)
        {
            await _employeeService.UpdateAsync(id, employee);

            return Ok(new
            {
                Message = "Employee updated successfully."
            });
        }

        [Authorize]
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Employee deleted successfully."
            });
        }
    }
}