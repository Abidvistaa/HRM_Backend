using ClosedXML.Excel;

namespace HRM_Backend.Service
{
    public interface IExcelService
    {
        Task<byte[]> ExportPayrollExcelAsync();
    }
    public class ExcelService : IExcelService
    {
        private readonly IPayrollService _payrollService;

        public ExcelService(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }
        public async Task<byte[]> ExportPayrollExcelAsync()
        {
            try
            {
                var payrolls = (await _payrollService.GetAllDTOAsync()).ToList();

                if (!payrolls.Any())
                    throw new KeyNotFoundException("No payroll records found.");

                using var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("Payroll List");

                // Title
                worksheet.Cell("A1").Value = "Payroll Report";
                worksheet.Range("A1:L1").Merge();
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Cell("A1").Style.Font.FontSize = 18;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Header Row
                int headerRow = 3;

                worksheet.Cell(headerRow, 1).Value = "ID";
                worksheet.Cell(headerRow, 2).Value = "Salary ID";
                worksheet.Cell(headerRow, 3).Value = "Employee";
                worksheet.Cell(headerRow, 4).Value = "Month";
                worksheet.Cell(headerRow, 5).Value = "Year";
                worksheet.Cell(headerRow, 6).Value = "Basic";
                worksheet.Cell(headerRow, 7).Value = "Bonus";
                worksheet.Cell(headerRow, 8).Value = "Deduction";
                worksheet.Cell(headerRow, 9).Value = "Tax";
                worksheet.Cell(headerRow, 10).Value = "Net Salary";
                worksheet.Cell(headerRow, 11).Value = "Status";
                worksheet.Cell(headerRow, 12).Value = "Action Date";

                var headerRange = worksheet.Range(headerRow, 1, headerRow, 12);

                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data
                int row = headerRow + 1;

                foreach (var p in payrolls)
                {
                    worksheet.Cell(row, 1).Value = p.Id;
                    worksheet.Cell(row, 2).Value = p.SalaryId;
                    worksheet.Cell(row, 3).Value = p.EmployeeName;
                    worksheet.Cell(row, 4).Value = p.PayrollMonthString;
                    worksheet.Cell(row, 5).Value = p.PayrollYear;
                    worksheet.Cell(row, 6).Value = p.BasicSalary;
                    worksheet.Cell(row, 7).Value = p.Bonus;
                    worksheet.Cell(row, 8).Value = p.Deduction;
                    worksheet.Cell(row, 9).Value = p.Tax;
                    worksheet.Cell(row, 10).Value = p.NetSalary;
                    worksheet.Cell(row, 11).Value = p.Status;
                    worksheet.Cell(row, 12).Value = p.ActionDate.ToString("dd MMM yyyy");

                    row++;
                }

                // Currency Format
                worksheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Column(8).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Column(9).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Column(10).Style.NumberFormat.Format = "#,##0.00";

                // Borders
                worksheet.Range(headerRow, 1, row - 1, 12)
                         .Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range(headerRow, 1, row - 1, 12)
                         .Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Center Alignment
                worksheet.Range(headerRow, 1, row - 1, 12)
                         .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Auto Fit
                worksheet.Columns().AdjustToContents();

                // Freeze Header
                //worksheet.SheetView.FreezeRows(headerRow);

                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                return stream.ToArray();
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while exporting the payroll Excel file.", ex);
            }
        }
    }
}
