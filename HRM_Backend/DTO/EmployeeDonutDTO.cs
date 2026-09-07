
using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.DTO
{
    public class EmployeeDonutDTO
    {
        public int TotalEmp { get; set; }
        public string Dept { get; set; } = string.Empty;
    }
}
