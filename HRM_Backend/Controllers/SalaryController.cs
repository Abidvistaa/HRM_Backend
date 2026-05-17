using HRM_Backend.Model;
using HRM_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [Authorize]
        [HttpGet("GetAllSalaries")]
        public async Task<IActionResult> GetAllSalaries()
        {
            try
            {
                var list = await _salaryService.GetAllAsync();

                return Ok(new
                {
                    success = true,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("GetSalaryById/{id}")]
        public async Task<IActionResult> GetSalaryById(int id)
        {
            try
            {
                var salary = await _salaryService.GetByIdAsync(id);
                return Ok(new
                {
                    success = true,
                    data = salary

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "HR,Finance")]
        [HttpPost("AddSalary")]
        public async Task<IActionResult> AddSalary(Salary salary)
        {
            try
            {
                await _salaryService.AddAsync(salary);

                return Ok(new
                {
                    success = true,
                    message = "Salary created successfully."
                });
            }
            catch(InvalidOperationException ex)
            {
                return NotFound( new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "HR,Finance")]
        [HttpPut("UpdateSalary/{id}")]
        public async Task<IActionResult> UpdateSalary(int id,Salary salary)
        {
            try
            {
                await _salaryService.UpdateAsync(id, salary);

                return Ok(new
                {
                    success = true,
                    message = "Salary updated successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }

        [Authorize(Roles = "HR,Finance")]
        [HttpDelete("DeleteSalary/{id}")]
        public async Task<IActionResult> DeleteSalary(int id)
        {
            try
            {
                await _salaryService.DeleteAsync(id);

                return Ok(new
                {
                    success = true, message = "Salary deleted successfully." 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new {success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetUpdatedSalaryList")]
        public async Task<IActionResult> GetUpdatedSalaryList()
        {
            try
            {
                var list = await _salaryService.GetUpdatedSalaryListAsync();

                return Ok(new
                {
                    success = true,
                    data = list
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}