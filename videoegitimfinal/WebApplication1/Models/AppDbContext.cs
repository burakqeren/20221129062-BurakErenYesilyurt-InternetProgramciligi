using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace videoegitimfinal.Models
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Egitim> Egitims { get; set; }
        public DbSet<Video> Videos { get; set; }

        public DbSet<Iletisim> Iletisims { get; set;}

        
    }


}
