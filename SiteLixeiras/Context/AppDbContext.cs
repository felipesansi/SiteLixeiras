using Microsoft.EntityFrameworkCore;

namespace SiteLixeiras.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
      
        // Defina suas entidades aqui
        // public DbSet<Entidade> Entidades { get; set; }
    }
}
