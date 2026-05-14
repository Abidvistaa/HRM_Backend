using HRM_Backend.DTO;
using HRM_Backend.Model;
using HRM_Backend.Repository;

namespace HRM_Backend.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task AddAsync(Employee obj);
        Task UpdateAsync(int id, Employee obj);
        Task DeleteAsync(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllAsync()
        {
            try
            {
                var list = await _employeeRepository.GetAllAsync();

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

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {model.Id} not found.");

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Phone = model.Phone;
                employee.AccountNumber = model.AccountNumber;
                employee.EmploymentStatus = model.EmploymentStatus;

                await _employeeRepository.UpdateAsync(employee);
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

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {id} not found.");

                await _employeeRepository.DeleteAsync(id);
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
