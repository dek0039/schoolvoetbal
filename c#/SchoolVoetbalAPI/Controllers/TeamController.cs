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
    public class TeamCreateRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class AssignMatchRequest
    {
        [Required]
        public int MatchId { get; set; }

        [Required]
        public int TeamId { get; set; }
    }

    public class DeleteTeamRequest
    {
        [Required]
        public int TeamId { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public TeamController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("getteams")]
        public IActionResult GetTeams()
        {
            var teams = _context.Team.ToList();
            var json = JsonConvert.SerializeObject(teams);

            return Ok(new { json });
        }

        [HttpGet("getteam/{id}")]
        public IActionResult GetTeam(int id)
        {
            var team = _context.Team.FirstOrDefault(t => t.Id == id);

            if (team == null)
            {
                return NotFound(new { message = "Team not found." });
            }

            var json = JsonConvert.SerializeObject(team);

            return Ok(new { json });
        }

        [HttpPost("addteam")]
        [Authorize]
        public IActionResult AddTeam([FromForm] TeamCreateRequest request)
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

            var team = new Team
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Team.Add(team);
            _context.SaveChanges();

            return Ok(new { message = "Sucess" });
        }

       
        [HttpPost("deleteteam")]
        [Authorize]
        public IActionResult DeleteTeam([FromForm] DeleteTeamRequest request)
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

            var team = _context.Team.Find(request.TeamId);
            if (team == null)
            {
                return NotFound(new { message = "Team not found." });
            }

            _context.Team.Remove(team);
            _context.SaveChanges();


            return Ok(new { message = "Sucess" });
        }
    }

}
