using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewTaskApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewTaskApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly TaskDatabaseContext _context;

        public AuthController(IConfiguration configuration, TaskDatabaseContext context)
        {
            _config = configuration;
            _context = context;
        }

        // Validate method to check user credentials
        [NonAction]
        public User ValidateUser(string email, string password)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [NonAction]
        public Admin ValidateAdmin(string email, string password)
        {
            return _context.Admins
                .FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate(string email, string password, string userType)
        {
            IActionResult response = Unauthorized();
            var token = string.Empty;

            if (userType.ToLower() == "user")
            {
                var user = ValidateUser(email, password);
                if (user != null)
                {
                    token = GenerateJwtToken(user.Email, user.UserName, "User");
                    return Ok(new { Token = token });
                }
            }
            else if (userType.ToLower() == "admin")
            {
                var admin = ValidateAdmin(email, password);
                if (admin != null)
                {
                    token = GenerateJwtToken(admin.Email, admin.Name, "Admin");
                    return Ok(new { Token = token });
                }
            }

            return response;
        }

        // Generate JWT token
        private string GenerateJwtToken(string email, string userName, string role)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            // Add user information to the JWT token claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)  // Add role claim to the token
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),  // Set the expiration time
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
