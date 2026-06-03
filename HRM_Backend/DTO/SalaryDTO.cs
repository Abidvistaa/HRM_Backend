
namespace HRM_Backend.DTO
{
    public class SalaryDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string IdPlusName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
