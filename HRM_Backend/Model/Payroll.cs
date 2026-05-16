using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.Model
{
    public class Payroll
    {
        public int Id { get; set; }
        [Required]
        public int SalaryId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int PayrollMonth { get; set; }
        [Required]
        public int PayrollYear { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        [Required]
        public decimal Tax { get; set; }
        [Required]
        public decimal NetSalary { get; set; }
        public string Status {  get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }

    }
}
