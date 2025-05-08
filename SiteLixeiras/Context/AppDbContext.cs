using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Models;

namespace SiteLixeiras.Context
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        // Construtor que recebe opções e as repassa para a classe base
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produtos> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<EnderecoEntrega> EnderecosEntregas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalhe> PedidoDetalhes { get; set; }
        public DbSet<CarrinhoCompraItem> CarrinhoCompraItens { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<DadosDropBox> DadosDropBox { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }


    }
}
