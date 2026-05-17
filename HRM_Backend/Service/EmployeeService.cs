using HRM_Backend.DTO;
using HRM_Backend.Model;
using HRM_Backend.Repository;

namespace HRM_Backend.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<IEnumerable<EmployeeDTO>> GetAllActiveAsync();
        Task<Employee> GetByIdAsync(int id);
        Task AddAsync(Employee obj);
        Task UpdateAsync(int id, Employee obj);
        Task DeleteAsync(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Salary> _salaryRepository;
        private readonly IRepository<Payroll> _payrollRepository;
        public EmployeeService(IRepository<Employee> employeeRepository, IRepository<Payroll> payrollRepository, IRepository<Salary> salaryRepository)
        {
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
            _salaryRepository = salaryRepository;
        }
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                var list = await _employeeRepository.GetAllAsync();

                return list.OrderByDescending(x=>x.Id);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employee list.", ex);
            }
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllActiveAsync()
        {
            try
            {
                var list = (await _employeeRepository.GetAllAsync()).Where(x=>x.EmploymentStatus == "Active");

                return list
                    .OrderByDescending(x => x.Id)
                    .Select(x => new EmployeeDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IdPlusName = x.Id + " - " + x.Name,
                        Email = x.Email,
                        Phone = x.Phone,
                        Department = x.Department,
                        AccountNumber = x.AccountNumber,
                        EmploymentStatus = x.EmploymentStatus,
                        HireDate = x.HireDate
                    });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employee list.", ex);
            }
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {id} not found.");

                return employee;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve employee.", ex);
            }
        }

        public async Task AddAsync(Employee employee)
        {
            try
            {

                await _employeeRepository.AddAsync(employee);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating employee.", ex);
            }
        }

        public async Task UpdateAsync(int id, Employee model)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                var salaries = await _salaryRepository.GetAllAsync();
                var payrolls = await _payrollRepository.GetAllAsync();

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {model.Id} not found.");

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Phone = model.Phone;
                employee.Department = model.Department;
                employee.AccountNumber = model.AccountNumber;
                employee.EmploymentStatus = model.EmploymentStatus;

                await _employeeRepository.UpdateAsync(employee);
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
                throw new Exception("An error occurred while updating employee.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {

                var employee = await _employeeRepository.GetByIdAsync(id);
                var salaries = await _salaryRepository.GetAllAsync();
                var payrolls = await _payrollRepository.GetAllAsync();

                bool isPresentInSalary = salaries.Any(x => x.EmployeeId == employee.Id);

                bool isPresentInPayroll = payrolls.Any(x => x.EmployeeId == employee.Id);

                if (isPresentInSalary)
                {
                    throw new InvalidOperationException(
                        $"This employee is already used in Salaries, it cannot be deleted."
                    );
                }

                if (isPresentInPayroll)
                {
                    throw new InvalidOperationException(
                        $"This employee is already used in Payroll, it cannot be deleted."
                    );
                }

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {id} not found.");

                await _employeeRepository.DeleteAsync(id);
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
                throw new Exception("An error occurred while deleting employee.", ex);
            }
        }

    }
}
