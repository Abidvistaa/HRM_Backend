using HRM_Backend.Model;
using HRM_Backend.Repository;

namespace HRM_Backend.Service
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(int id);
    }

    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        public RoleService(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            try
            {
                var list = await _roleRepository.GetAllAsync();

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve role list.", ex);
            }
        }
       
        public async Task<Role> GetByIdAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);

                if (role == null)
                    throw new KeyNotFoundException($"Role with ID {id} not found.");

                return role;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve role.", ex);
            }
        }
    }
}
