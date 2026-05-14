
using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.DTO
{
    public class PayrollDTO
    {
        public int Id { get; set; }
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int PayrollMonth { get; set; }
        public string PayrollMonthString { get; set; } = string.Empty;
        public int PayrollYear { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
    }
}
