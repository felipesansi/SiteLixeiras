using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Models;
namespace SiteLixeiras.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
      
        // Defina suas entidades aqui
        // public DbSet<Entidade> Entidades { get; set; }

        public DbSet<Produtos> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalhe> PedidoDetalhes { get; set; }

    }
}
