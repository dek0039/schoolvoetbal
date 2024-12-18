namespace SchoolVoetbalAPI.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public float Balance {  get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsPlayer { get; set; } = false;
        public int AssignedTeam { get; set; } = 0;
    }
}
