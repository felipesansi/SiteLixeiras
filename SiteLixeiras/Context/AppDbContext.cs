using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Models;

namespace SiteLixeiras.Context
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produtos> Produtos { get; set; }
        public DbSet<ProdutoKitItem> ProdutoKitItens { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<EnderecoEntrega> EnderecosEntregas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalhe> PedidoDetalhes { get; set; }
        public DbSet<CarrinhoCompraItem> CarrinhoCompraItens { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<DadosDropBox> DadosDropBox { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento ProdutoKitItem -> ProdutoKit (produto pai)
            modelBuilder.Entity<ProdutoKitItem>()
                .HasOne(p => p.ProdutoKit)
                .WithMany(p => p.ItensDoKit)
                .HasForeignKey(p => p.ProdutoKitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento ProdutoKitItem -> ProdutoFilho (produto componente)
            modelBuilder.Entity<ProdutoKitItem>()
                .HasOne(p => p.ProdutoFilho)
                .WithMany()
                .HasForeignKey(p => p.ProdutoFilhoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
