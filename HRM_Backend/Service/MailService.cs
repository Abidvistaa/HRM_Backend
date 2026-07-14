using HRM_Backend.DTO;
using HRM_Backend.Model;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace HRM_Backend.Service
{
    public interface IMailService
    {
        Task SendPayrollMailAsync();
    }
    public class MailService : IMailService
    {
        private readonly IPayrollService _payrollService;
        private readonly IPdfService _pdfService;
        private readonly IExcelService _excelService;
        private readonly IConfiguration _configuration;

        public MailService(
            IPayrollService payrollService,
            IPdfService pdfService,
            IExcelService excelService,
            IConfiguration configuration)
        {
            _payrollService = payrollService;
            _pdfService = pdfService;
            _excelService = excelService;
            _configuration = configuration;
        }

        public async Task SendPayrollMailAsync()
        {
            try
            {
                var payrolls = (await _payrollService.GetAllDTOAsync()).ToList();

                if (!payrolls.Any())
                    throw new KeyNotFoundException("No payroll records found.");

                // Read HTML Template
                string html = await File.ReadAllTextAsync("Templates/PayrollMailReport.html");

                // Build Table Rows
                StringBuilder rows = new StringBuilder();

                foreach (var p in payrolls)
                {
                    rows.Append($@"
                <tr>
                <td>{p.Id}</td>
                <td>{p.SalaryId}</td>
                <td>{p.EmployeeName}</td>
                <td>{p.PayrollMonthString}</td>
                <td>{p.PayrollYear}</td>
                <td>{p.BasicSalary:N2}</td>
                <td>{p.Bonus:N2}</td>
                <td>{p.Deduction:N2}</td>
                <td>{p.Tax}</td>
                <td>{p.NetSalary:N2}</td>
                <td>{p.Status}</td>
                <td>{p.ActionDate:dd MMM yyyy}</td>
                </tr>");
                }

                // Replace Placeholders
                html = html.Replace("{{PayrollRows}}", rows.ToString());
                html = html.Replace("{{TotalPayroll}}", payrolls.Count.ToString());
                html = html.Replace("{{GeneratedDate}}", DateTime.Now.ToString("dd MMM yyyy"));

                // Create Mail
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(_configuration["MailSettings:From"], "Intelligence Academy");

                mail.To.Add("mahassan22300@gmail.com");

                mail.Subject = "Payroll Report";

                mail.IsBodyHtml = true;

                mail.Body = html;

                // Attach PDF
                byte[] pdf = await _pdfService.ExportPayrollPdfAsync();

                mail.Attachments.Add(new Attachment(
                    new MemoryStream(pdf),
                    $"Payroll_List_{DateTime.Now:yyyyMMdd}.pdf",
                    "application/pdf"));

                //Attach Excel
                byte[] excel = await _excelService.ExportPayrollExcelAsync();

                mail.Attachments.Add(new Attachment(
                    new MemoryStream(excel),
                    $"Payroll_List_{DateTime.Now:yyyyMMdd}.xlsx",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

                // SMTP
                using SmtpClient smtp = new SmtpClient(
                    _configuration["MailSettings:Host"],
                    int.Parse(_configuration["MailSettings:Port"]));

                smtp.EnableSsl = true;

                smtp.Credentials = new NetworkCredential(
                    _configuration["MailSettings:UserName"],
                    _configuration["MailSettings:Password"]);

                await smtp.SendMailAsync(mail);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while sending the payroll email.", ex);
            }
        }
    }
}
