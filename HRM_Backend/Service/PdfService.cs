using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace HRM_Backend.Service
{
    public interface IPdfService
    {
        Task<byte[]> ExportPayrollPdfAsync();
    }
    public class PdfService : IPdfService
    {
        private readonly IPayrollService _payrollService;

        public PdfService(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        public async Task<byte[]> ExportPayrollPdfAsync()
        {
            try
            {
                var payrolls = (await _payrollService.GetAllDTOAsync()).ToList();

                if (!payrolls.Any())
                    throw new KeyNotFoundException("No payroll records found.");

                return Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(25);

                        page.Header()
                            .PaddingBottom(10)
                            .Text("Payroll List")
                            .FontSize(18)
                            .Bold();

                        page.Content()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1); // ID
                                    columns.RelativeColumn(1); // Salary ID
                                    columns.RelativeColumn(1); // Employee
                                    columns.RelativeColumn(1); // Month
                                    columns.RelativeColumn(1); // Year
                                    columns.RelativeColumn(1); // Basic
                                    columns.RelativeColumn(1); // Bonus
                                    columns.RelativeColumn(1); // Deduction
                                    columns.RelativeColumn(1); // Tax
                                    columns.RelativeColumn(1); // Net
                                    columns.RelativeColumn(1); // Status
                                    columns.RelativeColumn(1); // Date
                                });

                                table.Header(header =>
                                {
                                    void Header(string text)
                                    {
                                        header.Cell()
                                            .Border(0.5f)
                                            .Background(Colors.Blue.Lighten4)
                                            .Padding(4)
                                            .Text(text)
                                            .AlignCenter()
                                            .Bold()
                                            .FontSize(6);
                                    }

                                    Header("ID");
                                    Header("Salary ID");
                                    Header("Employee");
                                    Header("Month");
                                    Header("Year");
                                    Header("Basic");
                                    Header("Bonus");
                                    Header("Deduction");
                                    Header("Tax");
                                    Header("Net");
                                    Header("Status");
                                    Header("Action Date");
                                });

                                bool alternate = false;

                                foreach (var p in payrolls)
                                {
                                    alternate = !alternate;

                                    var bg = alternate
                                        ? Colors.White
                                        : Colors.Grey.Lighten4;

                                    void Cell(string text)
                                    {
                                        table.Cell()
                                            .Border(0.5f)
                                            .Background(bg)
                                            .Padding(4)
                                            .Text(text)
                                            .AlignCenter()
                                            .FontSize(6);
                                    }

                                    Cell(p.Id.ToString());
                                    Cell(p.SalaryId.ToString());
                                    Cell(p.EmployeeName);
                                    Cell(p.PayrollMonthString);
                                    Cell(p.PayrollYear.ToString());
                                    Cell(p.BasicSalary.ToString("N2"));
                                    Cell(p.Bonus.ToString("N2"));
                                    Cell(p.Deduction.ToString("N2"));
                                    Cell(p.Tax.ToString());
                                    Cell(p.NetSalary.ToString("N2"));
                                    Cell(p.Status);
                                    Cell(p.ActionDate.ToString("dd MMM yyyy"));
                                }
                            });

                        page.Footer()
                            .AlignRight()
                            .Text($"Generated: {DateTime.Now:dd MMM yyyy}");
                    });
                }).GeneratePdf();
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while exporting the payroll PDF.", ex);
            }
        }
    }
}
