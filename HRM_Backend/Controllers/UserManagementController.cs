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
        string GenerateJwtToken(string username)
        {
            var jwt = _config.GetSection("Jwt");

            var key = jwt["Key"];
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: "HRM",
                audience: "HRM",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login user)
        {
            bool Success = false;

            var objAdmin = (await _userService.GetAllAsync()).Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();
            var obj = (await _userService.GetAllAsync()).Where(x=>x.Username == user.Username && x.Password== hashPassword.Encode(user.Password)).FirstOrDefault();


            if (objAdmin != null)
            {
                Success = true;

                var token = GenerateJwtToken(user.Username);

                return Ok(new
                {
                    Success,
                    userName = user.Username,
                    token
                });
            }
            else if (obj != null)
            {
                Success = true;

                var token = GenerateJwtToken(user.Username);

                return Ok(new
                {
                    Success,
                    userName = user.Username,
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
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var list = await _userService.GetByIdAsync(id);

            return Ok(list);
        }


        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(User obj)
        {
            await _userService.AddAsync(obj);

            return Ok(new
            {
                Message = "User created successfully."
            });
        }

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User obj)
        {
            await _userService.UpdateAsync(obj);

            return Ok(new
            {
                Message = "User updated successfully."
            });
        }

        [Authorize]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);

            return Ok(new
            {
                Message = "User deleted successfully."
            });
        }
    }
}
