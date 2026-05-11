using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.Model
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
        [Required]
        public string AccountNumber { get; set; } = string.Empty;
        [Required]
        public string EmploymentStatus { get; set; } = string.Empty;
        [Required]
        public DateTime HireDate { get; set; }
    }
}
