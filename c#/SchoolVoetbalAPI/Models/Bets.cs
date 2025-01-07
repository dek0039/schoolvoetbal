namespace SchoolVoetbalAPI.Models
{
        public class Bets
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int MatchId { get; set; }
            public int WinningTeam { get; set; }
            public float Amount { get; set; }
        }
}
