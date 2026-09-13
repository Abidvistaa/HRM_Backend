
using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.DTO
{
    public class EmployeeDonutResponseDTO
    {
        public int GrossTotalEmp { get; set; }
        public int ActiveTotalEmp { get; set; }
        public IEnumerable<EmployeeDonutDTO> DepartmentInfo { get;set; }
    }
}
