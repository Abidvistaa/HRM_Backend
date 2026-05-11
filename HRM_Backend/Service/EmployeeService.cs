using HRM_Backend.Model;
using HRM_Backend.Repository;

namespace HRM_Backend.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task AddAsync(Employee obj);
        Task UpdateAsync(Employee obj);
        Task DeleteAsync(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                var list = await _employeeRepository.GetAllAsync();

                return list;
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

        public async Task UpdateAsync(Employee model)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(model.Id);

                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {model.Id} not found.");

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Phone = model.Phone;
                employee.Department = model.Department;
                employee.Position = model.Position;
                employee.AccountNumber = model.AccountNumber;
                employee.EmploymentStatus = model.EmploymentStatus;
                employee.HireDate = model.HireDate;

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
