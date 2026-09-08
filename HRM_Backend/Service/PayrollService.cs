using HRM_Backend.DTO;
using HRM_Backend.Model;
using HRM_Backend.Repository;
using System.Globalization;

namespace HRM_Backend.Service
{
    public interface IPayrollService
    {
        Task<IEnumerable<Payroll>> GetAllAsync();
        Task<Payroll> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<PayrollDTO>> GetAllDTOAsync();
        Task<int> GenerateMonthlyPayrollAsync(Payroll obj);
        Task<PayrollChartResponseDTO> GetMonthlyAmountsAsync();
    }

    public class PayrollService : IPayrollService
    {
        private readonly IRepository<Payroll> _payrollRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Salary> _salaryRepository;
        public PayrollService(IRepository<Payroll> payrollRepository, IRepository<Employee> employeeRepository, IRepository<Salary> salaryRepository)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _salaryRepository = salaryRepository;
        }
        public async Task<IEnumerable<Payroll>> GetAllAsync()
        {
            try
            {
                var list = await _payrollRepository.GetAllAsync();

                return list.OrderByDescending(x => x.ActionDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payroll list.", ex);
            }
        }

        public async Task<Payroll> GetByIdAsync(int id)
        {
            try
            {
                var payroll = await _payrollRepository.GetByIdAsync(id);

                if (payroll == null)
                    throw new KeyNotFoundException($"Payroll with ID {id} not found.");

                return payroll;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payroll.", ex);
            }
        }
        public async Task<IEnumerable<PayrollDTO>> GetAllDTOAsync()
        {
            try
            {
                var payrolls = await _payrollRepository.GetAllAsync();
                var employees = await _employeeRepository.GetAllAsync();
                var salaries = await _salaryRepository.GetAllAsync();

                var list = payrolls.Select(obj =>
                {
                    var employee = employees.FirstOrDefault(x => x.Id == obj.EmployeeId);

                    var specificSal = salaries.Where(x => x.Id == obj.SalaryId).FirstOrDefault();

                    return new PayrollDTO
                    {
                        Id = obj.Id,
                        SalaryId = obj.SalaryId,
                        EmployeeId = obj.EmployeeId,
                        EmployeeName = employee?.Name ?? "",
                        PayrollMonthString = new DateTime(1, obj.PayrollMonth, 1).ToString("MMM"),
                        PayrollMonth = obj.PayrollMonth,
                        PayrollYear = obj.PayrollYear,
                        BasicSalary = specificSal.BasicSalary,
                        Bonus = specificSal.Bonus,
                        Deduction = specificSal.Deduction,
                        Tax = obj.Tax,
                        NetSalary = obj.NetSalary,
                        Status = obj.Status,
                        ActionDate = obj.ActionDate
                    };
                });

                return list.OrderByDescending(x => x.PayrollMonth);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payroll list.", ex);
            }
        }

        public async Task<int> GenerateMonthlyPayrollAsync(Payroll model)
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();
                var salaries = await _salaryRepository.GetAllAsync();
                var payrolls = await _payrollRepository.GetAllAsync();

                var payrollDate = new DateTime(
                    model.PayrollYear,
                    model.PayrollMonth,
                    1
                );

                // latest salary
                var latestSalaries = salaries
                    .Where(x => x.EffectiveDate <= payrollDate) // only salary effective before or on payroll month
                    .GroupBy(x => x.EmployeeId)
                    .Select(g => g.OrderByDescending(x => x.EffectiveDate).FirstOrDefault());

                int generatedCount = 0;

                foreach (var obj in latestSalaries)
                {
                    // already generated?
                    bool exists = payrolls.Any(x =>
                        x.EmployeeId == obj.EmployeeId &&
                        x.PayrollMonth == model.PayrollMonth &&
                        x.PayrollYear == model.PayrollYear);

                    if (exists)
                        continue; //skip for that employee

                    var employee = employees.Where(x => x.Id == obj.EmployeeId).FirstOrDefault();

                    decimal basicSalary = obj.BasicSalary;

                    // gross salary
                    decimal grossSalary = basicSalary + obj.Bonus - obj.Deduction;

                    // tax from rule
                    decimal taxAmount = CalculateTax(grossSalary);

                    // net salary
                    decimal netSalary = grossSalary - taxAmount;

                    var payroll = new Payroll
                    {
                        SalaryId = obj.Id,
                        EmployeeId = employee.Id,
                        PayrollMonth = model.PayrollMonth,
                        PayrollYear = model.PayrollYear,
                        Tax = taxAmount,
                        NetSalary = netSalary,
                        Status = "Generated",
                        ActionDate = DateTime.Now
                    };

                    await _payrollRepository.AddAsync(payroll);

                    generatedCount++;
                }

                var month = new DateTime(1, model.PayrollMonth, 1).ToString("MMM");
                if (generatedCount == 0)
                {
                    throw new InvalidOperationException($"Already generated payrolls for {month}, {model.PayrollYear}.");
                }

                return generatedCount;
            }

            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate payroll.", ex);
            }
        }

        private decimal CalculateTax(decimal grossSalary)
        {
            //All the rules are estimated only

            decimal taxPercent = 0;

            // yearly estimation
            decimal yearlySalary = grossSalary * 12;

            // estimated tax rules
            if (yearlySalary <= 600000)
            {
                taxPercent = 0;
            }
            else if (yearlySalary <= 1000000)
            {
                taxPercent = 5;
            }
            else if (yearlySalary <= 5000000)
            {
                taxPercent = 10;
            }
            else
            {
                taxPercent = 15;
            }

            return (grossSalary * taxPercent) / 100;
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var payroll = await _payrollRepository.GetByIdAsync(id);

                if (payroll == null)
                    throw new KeyNotFoundException($"Payroll with ID {id} not found.");

                await _payrollRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting payroll.", ex);
            }
        }

        public async Task<PayrollChartResponseDTO> GetMonthlyAmountsAsync()
        {
            try
            {
                var list = (await _payrollRepository.GetAllAsync()).ToList();

                var amounts = list
                    .GroupBy(x => x.PayrollMonth)
                    .Select(g => new PayrollChartDTO
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat
                    .GetAbbreviatedMonthName(g.Key),

                        TotalAmount = g.Sum(x => x.NetSalary)
                    })
                    .ToList();

                return new PayrollChartResponseDTO
                {
                    GrossTotalAmount = amounts.Sum(x=>x.TotalAmount),
                    MonthlyChartInfo = amounts
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employee department data.", ex);
            }
        }
    }
}
