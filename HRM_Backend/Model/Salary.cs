using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.Model
{
    public class Salary
    {
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public decimal BasicSalary { get; set; }
        [Required]
        public DateTime EffectiveDate { get; set; }

    }
}
