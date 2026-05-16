using HRM_Backend.Model;
using HRM_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }


        [HttpGet("GetAllPayrolls")]
        public async Task<IActionResult> GetAllPayrolls()
        {
            try
            {
                var list = await _payrollService.GetAllDTOAsync();

                return Ok( new 
                { 
                    success = true,
                    data = list 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetPayrollById/{id}")]
        public async Task<IActionResult> GetPayrollById(int id)
        {
            try
            {
                var payroll = await _payrollService.GetByIdAsync(id);

                return Ok(new
{
                    success = false,
                    data = payroll
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [Authorize(Roles = "HR")]
        [HttpPost("AutoGeneratePayrollMonthly")]
        public async Task<IActionResult> AutoGeneratePayrollMonthly(Payroll payroll)
        {
            try
            {
                int generatedCount = await _payrollService.GenerateMonthlyPayrollAsync(payroll);

                var month = new DateTime(1, payroll.PayrollMonth, 1).ToString("MMM");

                if (generatedCount == 0)
                {
                    return Ok(new
                    {
                        success = false,
                        message = $"Already generated payrolls for {month}, {payroll.PayrollYear}."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"{generatedCount} payrolls generated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "HR")]
        [HttpDelete("DeletePayroll/{id}")]
        public async Task<IActionResult> DeletePayroll(int id)
        {
            try
            {
                await _payrollService.DeleteAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Payroll deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }
    }
}