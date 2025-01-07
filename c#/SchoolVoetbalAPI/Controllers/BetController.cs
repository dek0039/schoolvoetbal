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
    public class CreateBetRequest
    {
        public int MatchId { get; set; }
        public int WinningTeam { get; set; }
        public float Amount { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class BetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BetController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("getbets")]
        [Authorize]
        public IActionResult GetBets()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var userId = _context.Users.FirstOrDefault(u => u.Username == username).Id;

            var bets = _context.Bets.Where(b => b.UserId == userId).ToList();

            if (bets == null || !bets.Any())
            {
                return NotFound(new { message = "No bets found for the user." });
            }

            var json = JsonConvert.SerializeObject(bets);
            return Ok(new { json });
        }

        [HttpPost("createbet")]
        [Authorize]
        public IActionResult CreateBet([FromForm] CreateBetRequest request)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            if (request == null)
            {
                return BadRequest(new { message = "Invalid bet data." });
            }

            if (user.Balance < request.Amount)
            {
                return BadRequest(new { message = "Insufficient balance for this bet." });
            }

            user.Balance -= request.Amount;

            var bet = new Bets
            {
                Amount = request.Amount,
                WinningTeam = request.WinningTeam,
                UserId = user.Id,
                MatchId = request.MatchId,
            };

            _context.Bets.Add(bet);
            _context.SaveChanges();

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(new { message = "Bet created successfully", bet });
        }

        [HttpPost("updatebets")]
        [Authorize]
        public IActionResult UpdateBets()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            var bets = _context.Bets.Where(b => b.UserId == user.Id).ToList();
            if (bets == null || !bets.Any())
            {
                return NotFound(new { message = "No bets found for the user." });
            }
            foreach (var bet in bets)
            {
                var match = _context.Match.FirstOrDefault(m => m.Id == bet.MatchId);
                if (match == null)
                {
                    continue;
                }

                if (match.DateTime < DateTime.Now)
                {                 
                    var winningTeam = match.ScoreTeam1 > match.ScoreTeam2 ? match.AssignedTeam1 : match.AssignedTeam2;
              
                    if (bet.WinningTeam == winningTeam)
                    {
                        bet.Amount *= 2;
                    }
                    else if (bet.WinningTeam == 0) 
                    {
                        bet.Amount *= 1.5f;
                    }
                    else
                    {
                        bet.Amount = 0;
                    }
                    user.Balance += bet.Amount;

                    _context.Bets.Update(bet);
                }
            }
            _context.SaveChanges();

            var json = JsonConvert.SerializeObject(bets);
            return Ok(new { json });
        }

    }

}
