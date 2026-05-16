using HRM_Backend.DTO;
using HRM_Backend.Model;
using HRM_Backend.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;

namespace HRM_Backend.Service
{
    public interface ISalaryService
    {
        Task<IEnumerable<SalaryDTO>> GetAllAsync();
        Task<Salary> GetByIdAsync(int id);
        Task AddAsync(Salary obj);
        Task UpdateAsync(int id, Salary obj);
        Task DeleteAsync(int id);
        Task<IEnumerable<SalaryDTO>> GetUpdatedSalaryListAsync();
    }

    public class SalaryService : ISalaryService
    {
        private readonly IRepository<Salary> _salaryRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Payroll> _payrollRepository;
        public SalaryService(IRepository<Salary> salaryRepository, IRepository<Employee> employeeRepository, IRepository<Payroll> payrollRepository)
        {
            _salaryRepository = salaryRepository;
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
        }

        public async Task<IEnumerable<SalaryDTO>> GetAllAsync()
        {
            try
            {
                var salaries = await _salaryRepository.GetAllAsync();
                var employees = await _employeeRepository.GetAllAsync();

                var list = salaries.Select(obj =>
                {
                    var employee = employees.FirstOrDefault(x => x.Id == obj.EmployeeId);

                     return new SalaryDTO
                     {
                         Id = obj.Id,
                         EmployeeId = obj.EmployeeId,
                         EmployeeName = employee?.Name ?? "",
                         BasicSalary = obj.BasicSalary,
                         EffectiveDate = obj.EffectiveDate
                     };

                });

                return list.OrderByDescending(x=>x.EffectiveDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve salary list.", ex);
            }
        }

        public async Task<Salary> GetByIdAsync(int id)
        {
            try
            {
                var salary = await _salaryRepository.GetByIdAsync(id);

                if (salary == null)
                    throw new KeyNotFoundException($"Salary with ID {id} not found.");

                return salary;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve salary.", ex);
            }
        }

        public async Task AddAsync(Salary salary)
        {
            try
            {
                await _salaryRepository.AddAsync(salary);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating salary.", ex);
            }
        }

        public async Task UpdateAsync(int id, Salary model)
        {
            try
            {
                var salary = await _salaryRepository.GetByIdAsync(id);

                var payrolls = await _payrollRepository.GetAllAsync();
                var employee = (await _employeeRepository.GetAllAsync()).Where(x => x.Id == salary.EmployeeId).FirstOrDefault();

                bool isPresentInPayroll = payrolls.Any(x => x.EmployeeId == employee.Id);

                if (isPresentInPayroll)
                {
                    throw new InvalidOperationException(
                        $"This salary is already used in Payroll, it cannot be updated."
                    );
                }

                if (salary == null)
                    throw new KeyNotFoundException($"Salary with ID {model.Id} not found.");

                salary.BasicSalary = model.BasicSalary;
                salary.EffectiveDate = model.EffectiveDate;


                await _salaryRepository.UpdateAsync(salary);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating salary.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {

                var salary = await _salaryRepository.GetByIdAsync(id);

                var payrolls = await _payrollRepository.GetAllAsync();
                var employee = (await _employeeRepository.GetAllAsync()).Where(x=>x.Id == salary.EmployeeId).FirstOrDefault();

                bool isPresentInPayroll = payrolls.Any(x => x.EmployeeId == employee.Id);

                if (isPresentInPayroll)
                {
                    throw new InvalidOperationException(
                        $"This salary is already used in Payroll, it cannot be deleted."
                    );
                }

                if (salary == null)
                    throw new KeyNotFoundException($"Salary with ID {id} not found.");

                await _salaryRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting salary.", ex);
            }
        }
        public async Task<IEnumerable<SalaryDTO>> GetUpdatedSalaryListAsync()
        {
            try
            {
                var salaries = await _salaryRepository.GetAllAsync();
                var employees = await _employeeRepository.GetAllAsync();

                var latestSalaries = salaries
                    .GroupBy(x => x.EmployeeId)
                    .Select(g => g.OrderByDescending(x => x.EffectiveDate).FirstOrDefault());

                var list = latestSalaries.Select(obj =>
                {
                    var employee = employees.FirstOrDefault(x => x.Id == obj.EmployeeId);

                    return new SalaryDTO
                    {
                        Id = obj.Id,
                        EmployeeId = obj.EmployeeId,
                        IdPlusName = employee.Id + " - " + employee.Name,
                        EmployeeName = employee?.Name ?? "",
                        BasicSalary = obj.BasicSalary,
                        EffectiveDate = obj.EffectiveDate
                    };
                });

                return list.OrderByDescending(x => x.EffectiveDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve salary list.", ex);
            }
        }

    }
}
