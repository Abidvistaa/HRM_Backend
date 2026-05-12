using HRM_Backend.Common;
using HRM_Backend.DTO;
using HRM_Backend.Model;
using HRM_Backend.Repository;

namespace HRM_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User obj);
        Task UpdateAsync(User obj);
        Task DeleteAsync(int id);
        Task<IEnumerable<UserDTO>> GetUsersWithRolesAsync();
    }

    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        HashPassword hashpassword = new HashPassword();
        public UserService(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var list = await _userRepository.GetAllAsync();

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve user list.", ex);
            }
        }
        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");

                return user;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve user.", ex);
            }
        }

        public async Task AddAsync(User user)
        {
            try
            {
                user.Password = hashpassword.Encode(user.Password);
                user.ActionDate = DateTime.Now; 
                await _userRepository.AddAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating user.", ex);
            }
        }

        public async Task UpdateAsync(User obj)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(obj.Id);

                if (user == null)
                    throw new KeyNotFoundException($"User with ID {obj.Id} not found.");

                user.UserName = obj.UserName;
                user.Password = hashpassword.Encode(obj.Password);
                user.RoleId = obj.RoleId;
                user.ActionDate = DateTime.Now;

                await _userRepository.UpdateAsync(user);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating user.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");

                await _userRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting user.", ex);
            }
        }

        public async Task<IEnumerable<UserDTO>> GetUsersWithRolesAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                var roles = await _roleRepository.GetAllAsync();

                var userList = users.Select(user => new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    RoleId = user.RoleId,
                    RoleName = roles.FirstOrDefault(r => r.Id == user.RoleId)?.RoleName ?? "",
                    ActionDate = user.ActionDate
                });

                return userList;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve user list.", ex);
            }
        }
    }
}
