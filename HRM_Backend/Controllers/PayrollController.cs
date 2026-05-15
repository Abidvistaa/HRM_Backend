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
            var list = await _payrollService.GetAllDTOAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("GetPayrollById/{id}")]
        public async Task<IActionResult> GetPayrollById(int id)
        {
            var payroll = await _payrollService.GetByIdDTOAsync(id);

            return Ok(payroll);
        }


        [Authorize(Roles = "HR")]
        [HttpPost("AddPayroll")]
        public async Task<IActionResult> AddPayroll(Payroll payroll)
        {
            var existing = (await _payrollService.GetAllAsync())
                .FirstOrDefault(x =>
                    x.EmployeeId == payroll.EmployeeId &&
                    x.PayrollMonth == payroll.PayrollMonth &&
                    x.PayrollYear == payroll.PayrollYear);

            if (existing != null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Payroll already exists for this month."
                });
            }

            await _payrollService.AddAsync(payroll);

            return Ok(new
            {
                Success = true,
                Message = "Payroll created successfully."
            });
        }

        [Authorize(Roles = "HR")]
        [HttpPut("UpdatePayroll/{id}")]
        public async Task<IActionResult> UpdatePayroll(int id,Payroll payroll)
        {
            await _payrollService.UpdateAsync(id, payroll);

            return Ok(new
            {
                Message = "Payroll updated successfully."
            });
        }

        [Authorize]
        [HttpDelete("DeletePayroll/{id}")]
        public async Task<IActionResult> DeletePayroll(int id)
        {
            await _payrollService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Payroll deleted successfully."
            });
        }

        [HttpPost("AutoGeneratePayrollMonthly")]
        public async Task<IActionResult> AutoGeneratePayrollMonthly(Payroll payroll)
        {
            int generatedCount = await _payrollService.GenerateMonthlyPayrollAsync(payroll);
            
            if(generatedCount == 0)
            {
                return Ok(new
                {
                    Success = false,
                    Message = $"{generatedCount} payrolls. So, no payroll has been generated.",
                    GeneratedCount = generatedCount
                });
            }

            return Ok(new
            {
                Success = true,
                Message = $"{generatedCount} payrolls generated successfully.",
                GeneratedCount = generatedCount
            });
        }
    }
}