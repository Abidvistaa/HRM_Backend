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
        private readonly IPdfService _pdfService;
        private readonly IExcelService _excelService;
        private readonly IMailService _mailService;
        public PayrollController(IPayrollService payrollService, IPdfService pdfService, IExcelService excelService, IMailService mailService )
        {
            _payrollService = payrollService;
            _pdfService = pdfService;
            _excelService = excelService;
            _mailService = mailService;
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

                return Ok(new
                {
                    success = true,
                    message = $"{generatedCount} payrolls generated successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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

        [Authorize(Roles = "HR")]
        [HttpGet("ExportPayrollPdf")]
        public async Task<IActionResult> ExportPayrollPdf()
        {
            try
            {
                var pdf = await _pdfService.ExportPayrollPdfAsync();

                return File(
                    pdf,
                    "application/pdf",
                    $"Payroll-List_{DateTime.Now:dd MMM yyyy}.pdf");
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
        [HttpGet("ExportPayrollExcel")]
        public async Task<IActionResult> ExportPayrollExcel()
        {
            var file = await _excelService.ExportPayrollExcelAsync();

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Payroll_List_{DateTime.Now:dd MMM yyyy}.xlsx");
        }

        [Authorize(Roles = "HR")]
        [HttpPost("SendPayrollMail")]
        public async Task<IActionResult> SendPayrollMail()
        {
            try
            {
                await _mailService.SendPayrollMailAsync();

                return Ok(new
                {
                    success = true,
                    message = "Payroll mail sent successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}