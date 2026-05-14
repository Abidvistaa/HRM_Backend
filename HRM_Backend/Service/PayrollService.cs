using HRM_Backend.DTO;
using HRM_Backend.Model;
using HRM_Backend.Repository;

namespace HRM_Backend.Service
{
    public interface IPayrollService
    {
        Task<IEnumerable<Payroll>> GetAllAsync();
        Task<Payroll> GetByIdAsync(int id);
        Task AddAsync(Payroll obj);
        Task UpdateAsync(int id, Payroll obj);
        Task DeleteAsync(int id);
        Task<IEnumerable<PayrollDTO>> GetAllDTOAsync();
        Task<PayrollDTO> GetByIdDTOAsync(int id);
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

        public async Task AddAsync(Payroll payroll)
        {
            try
            {
                payroll.ActionDate = DateTime.Now;
                payroll.Status = "Processing";
                await _payrollRepository.AddAsync(payroll);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating payroll.", ex);
            }
        }

        public async Task UpdateAsync(int id, Payroll model)
        {
            try
            {
                var payroll = await _payrollRepository.GetByIdAsync(id);

                if (payroll == null)
                    throw new KeyNotFoundException($"Payroll with ID {model.Id} not found.");

                payroll.PayrollMonth = model.PayrollMonth;
                payroll.PayrollYear = model.PayrollYear;
                payroll.Bonus = model.Bonus;
                payroll.Deduction = model.Deduction;
                payroll.Tax = model.Tax;
                payroll.NetSalary = model.NetSalary;
                payroll.Status = payroll.Status;
                payroll.ActionDate = DateTime.Now;

                await _payrollRepository.UpdateAsync(payroll);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating payroll.", ex);
            }
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

                    var salary = salaries
                        .Where(x => x.EmployeeId == obj.EmployeeId)
                        .OrderByDescending(x => x.EffectiveDate)
                        .FirstOrDefault();

                    return new PayrollDTO
                    {
                        Id = obj.Id,
                        EmployeeId = obj.EmployeeId,
                        EmployeeName = employee?.Id + " - " + employee?.Name ?? "",
                        PayrollMonthString = new DateTime(1, obj.PayrollMonth, 1).ToString("MMM"),
                        PayrollYear = obj.PayrollYear,
                        BasicSalary = salary?.BasicSalary ?? 0,
                        Bonus = obj.Bonus,
                        Deduction = obj.Deduction,
                        Tax = obj.Tax,
                        NetSalary = obj.NetSalary,
                        Status = obj.Status,
                        ActionDate = obj.ActionDate
                    };
                });

                return list.OrderByDescending(x => x.ActionDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payroll list.", ex);
            }
        }
        public async Task<PayrollDTO> GetByIdDTOAsync(int id)
        {
            try
            {
                var payroll = await _payrollRepository.GetByIdAsync(id);

                if (payroll == null)
                    throw new KeyNotFoundException($"Payroll with ID {id} not found.");

                var employee = await _employeeRepository.GetByIdAsync(payroll.EmployeeId);

                var salary = (await _salaryRepository.GetAllAsync())
                    .Where(x => x.EmployeeId == payroll.EmployeeId)
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();

                var dto = new PayrollDTO
                {
                    Id = payroll.Id,
                    SalaryId = salary.Id,
                    EmployeeId = payroll.EmployeeId,
                    EmployeeName = employee.Id + " - " + employee.Name,
                    PayrollMonthString = new DateTime(1, payroll.PayrollMonth, 1).ToString("MMM"),
                    PayrollMonth = payroll.PayrollMonth,
                    PayrollYear = payroll.PayrollYear,
                    BasicSalary = salary?.BasicSalary ?? 0,
                    Bonus = payroll.Bonus,
                    Deduction = payroll.Deduction,
                    Tax = payroll.Tax,
                    NetSalary = payroll.NetSalary,
                    Status = payroll.Status,
                    ActionDate = payroll.ActionDate
                };

                return dto;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payroll DTO.", ex);
            }
        }

    }
}
