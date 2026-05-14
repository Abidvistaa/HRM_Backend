
namespace HRM_Backend.DTO
{
    public class SalaryDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
