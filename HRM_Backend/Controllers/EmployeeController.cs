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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _employeeService.GetAllAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            await _employeeService.AddAsync(employee);

            return Ok(new
            {
                Message = "Employee created successfully."
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Employee employee)
        {
            await _employeeService.UpdateAsync(employee);

            return Ok(new
            {
                Message = "Employee updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Employee deleted successfully."
            });
        }
    }
}