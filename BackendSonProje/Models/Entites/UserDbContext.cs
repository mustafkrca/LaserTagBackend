using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace BackendSonProje.Models.Entites
{
    public class UserDbContext : DbContext
    {
        public UserDbContext() : base()
        {

        }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

    }
}
