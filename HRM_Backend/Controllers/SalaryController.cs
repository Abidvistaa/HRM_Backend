using HRM_Backend.Model;
using HRM_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public SalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }


        [HttpGet("GetAllSalaries")]
        public async Task<IActionResult> GetAllSalaries()
        {
            var list = await _salaryService.GetAllAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("GetSalaryById/{id}")]
        public async Task<IActionResult> GetSalaryById(int id)
        {
            var salary = await _salaryService.GetByIdAsync(id);

            return Ok(salary);
        }

        [Authorize]
        [HttpPost("AddSalary")]
        public async Task<IActionResult> AddSalary(Salary salary)
        {
            await _salaryService.AddAsync(salary);

            return Ok(new
            {
                Message = "Salary created successfully."
            });
        }

        [Authorize]
        [HttpPut("UpdateSalary/{id}")]
        public async Task<IActionResult> UpdateSalary(int id,Salary salary)
        {
            await _salaryService.UpdateAsync(id, salary);

            return Ok(new
            {
                Message = "Salary updated successfully."
            });
        }

        [Authorize]
        [HttpDelete("DeleteSalary/{id}")]
        public async Task<IActionResult> DeleteSalary(int id)
        {
            await _salaryService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Salary deleted successfully."
            });
        }

        [Authorize]
        [HttpGet("GetUpdatedSalaryList")]
        public async Task<IActionResult> GetUpdatedSalaryList()
        {
            var list = await _salaryService.GetUpdatedSalaryListAsync();

            return Ok(list);
        }
    }
}