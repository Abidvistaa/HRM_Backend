using HRM_Backend.Common;
using HRM_Backend.Model;
using HRM_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _config;
        HashPassword hashPassword = new HashPassword();
        public UserManagementController(IUserService userService, IRoleService roleService, IConfiguration config)
        {
            _userService = userService;
            _roleService = roleService;
            _config = config;
        }
        string GenerateJwtToken(string username, string role)
        {
            var jwt = _config.GetSection("Jwt");

            var key = jwt["Key"];
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: "HRM",
                audience: "HRM",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login user)
        {
            try
            {

            }
            catch (Exception ex) 
            {

            }
            bool Success = false;

            var objAdmin = (await _userService.GetAllAsync()).Where(x => x.UserName == user.UserName && x.Password == user.Password).FirstOrDefault();
            var obj = (await _userService.GetAllAsync()).Where(x=>x.UserName == user.UserName && x.Password== hashPassword.Encode(user.Password)).FirstOrDefault();

            if (objAdmin != null)
            {
                Success = true;

                var role = await _roleService.GetByIdAsync(objAdmin.RoleId);

                var token = GenerateJwtToken(user.UserName, role.RoleName);

                return Ok(new
                {
                    Success,
                    userName = user.UserName,
                    roleName = role.RoleName,
                    token
                });
            }
            else if (obj != null)
            {
                Success = true;

                var role = await _roleService.GetByIdAsync(obj.RoleId);

                var token = GenerateJwtToken(user.UserName, role.RoleName);

                return Ok(new
                {
                    Success,
                    userName = user.UserName,
                    roleName = role.RoleName,
                    token
                });
            }
            else
            {
                return Ok(new
                {
                    Success,
                    Message = "Please Enter Valid User Info!"
                });
            }
        }

        [Authorize]
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var list = await _roleService.GetAllAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var list = await _userService.GetAllAsync();

            return Ok(list);
        }

        [Authorize]
        [HttpGet("GetAllUsersWithRoles")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            try
            {
                var list = await _userService.GetUsersWithRolesAsync();

                return Ok(new
                {
                    success = true,
                    data = list,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var obj = await _userService.GetByIdAsync(id);

                return Ok(new
                {
                    success = true,
                    data = obj,
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound( new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(User obj)
        {
            try
            {
                await _userService.AddAsync(obj);

                return Ok(new
                {
                    success = true,
                    message = "User created successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User obj)
        {
            try
            {
                await _userService.UpdateAsync(obj);

                return Ok(new
                {
                    success = true,
                    message = "User updated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "User deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
