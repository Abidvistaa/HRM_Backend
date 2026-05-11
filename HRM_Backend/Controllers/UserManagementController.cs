using LMS.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : Controller
    {
        private readonly IConfiguration _config;

        public UserManagementController(IConfiguration config)
        {
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
            if (user.Username == "admin" && user.Password == "1234")
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
    }
}
