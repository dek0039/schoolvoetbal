using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SchoolVoetbalAPI.Data;
using SchoolVoetbalAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace SchoolVoetbalAPI.Controllers
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [EmailAddress] 
        public string Email { get; set; }
    }


    public class LoginRequest
    {
        [Required]
        public string UsernameOrEmail { get; set; }  

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] RegisterRequest request)
        {
            // Check if the username or email already exists
            if (_context.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
            {
                return BadRequest(new { message = "Username or email already exists." });
            }

            var user = new User
            {
                Username = request.Username,
                Password = _passwordHasher.HashPassword(null, request.Password),
                Email = request.Email,
                Balance = 100 //Bonus Balance
         
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.UsernameOrEmail);

            if (user == null)
            {
                user = _context.Users.FirstOrDefault(u => u.Email == request.UsernameOrEmail);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }
            }


            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = GenerateJwtToken(request.UsernameOrEmail);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("data")]
        [Authorize]
        public IActionResult Data()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !user.IsAdmin)
            {
                return Ok(new { message = "User is invalid." });
            }

            user.Password = "private";
            string jsonData = JsonConvert.SerializeObject(user);
            return Ok(new { jsonData });
        }

        [HttpPost("isadmin")]
        [Authorize]
        public IActionResult IsAdmin()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !user.IsAdmin) 
            {
                return Ok(new { message = "User is not an admin." });
            }

            return Ok(new { isAdmin = true });
        }
    }

}
