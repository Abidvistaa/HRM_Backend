

namespace HRM_Backend.DTO
{
    public class PayrollChartResponseDTO
    {
        public decimal GrossTotalAmount { get; set; }
        public IEnumerable<PayrollChartDTO> MonthlyChartInfo { get; set; }
    }
}
