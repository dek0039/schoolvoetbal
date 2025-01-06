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
using Microsoft.EntityFrameworkCore.Storage;

namespace SchoolVoetbalAPI.Controllers
{
    public class CreateTournementRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TournementController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public TournementController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("gettournements")]
        public IActionResult GetTeams()
        {

            var teams = _context.Tournement.ToList();
            var json = JsonConvert.SerializeObject(teams);

            return Ok(new { json });
        }

        [HttpPost("createtournement")]
        [Authorize]
        public IActionResult CreateMatch([FromForm] CreateTournementRequest request)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !user.IsAdmin)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var tournement = new Tournement
            {
                Name = request.Name,
                Description = request.Description,
            };

            _context.Tournement.Add(tournement);
            _context.SaveChanges();

            return Ok(new { message = "Sucess" });
        }
    }

}
