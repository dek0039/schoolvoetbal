namespace SchoolVoetbalAPI.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DateTime { get; set; }
        public int ScoreTeam1 {  get; set; }
        public int ScoreTeam2 { get; set; }
        public int AssignedTeam1 { get; set; }
        public int AssignedTeam2 { get; set; }
    }
}
