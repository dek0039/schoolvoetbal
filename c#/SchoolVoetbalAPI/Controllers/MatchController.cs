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
    public class CreateMatchRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DateTime { get; set; }
        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }
        public Team AssignedTeam1 { get; set; }
        public Team AssignedTeam2 { get; set; }
        public int AssignedTournement { get; set; }
    }

    public class UpdateScoreRequest
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public int Score {  get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MatchController(ApplicationDbContext context, IConfiguration configuration)
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

        [HttpPost("creatematch")]
        [Authorize]
        public IActionResult CreateMatch([FromForm] CreateMatchRequest request)
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

            var match = new Match
            {
                Name = request.Name,
                Description = request.Description,
                DateTime = request.DateTime,
                AssignedTeam1 = request.AssignedTeam1,
                AssignedTeam2 = request.AssignedTeam2,
                ScoreTeam1 = request.ScoreTeam1,
                ScoreTeam2 = request.ScoreTeam2,
                AssignedTournement = request.AssignedTournement,
            };

            _context.Match.Add(match);
            _context.SaveChanges();     

            return Ok(new { message = "Sucess" });
        }


        [HttpPost("updatescore")]
        [Authorize]
        public IActionResult UpdateScore([FromForm] UpdateScoreRequest request)
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

            var match = _context.Match.Find(request.MatchId);
            if (match == null)
            {
                return NotFound(new { message = "Match not found." });
            }

            if (match.AssignedTeam1.Id == request.TeamId)
            {
                match.ScoreTeam1 = request.Score;
            }

            if (match.AssignedTeam2.Id == request.TeamId)
            {
                match.ScoreTeam2 = request.Score;
            }

            _context.Match.Update(match);
            _context.SaveChanges();


            return Ok(new { message = "Sucess" });
        }


        [HttpPost("getmatches")]
        public IActionResult GetMatches()
        {
            var matches = _context.Match.ToList();
            var json = JsonConvert.SerializeObject(matches);

            return Ok(new { json });
        }
    }

}
