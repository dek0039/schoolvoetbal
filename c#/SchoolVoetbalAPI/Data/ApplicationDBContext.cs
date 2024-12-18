using SchoolVoetbalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolVoetbalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<Tournement> Tournement { get; set; }
        public DbSet<Team> Team { get; set; }
    }
}
