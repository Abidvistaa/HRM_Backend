
using System.ComponentModel.DataAnnotations;

namespace HRM_Backend.DTO
{
    public class PayrollChartDTO
    {
        public decimal TotalAmount { get; set; }
        public string Month { get; set; } = string.Empty;
    }
}
