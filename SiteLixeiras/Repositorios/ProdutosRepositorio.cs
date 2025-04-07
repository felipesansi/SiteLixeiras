using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;

namespace SiteLixeiras.Repositorios
{
    public class ProdutosRepositorio:IProdutosRepositorio
    {
        private readonly AppDbContext _context;

        public ProdutosRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Produtos> produtos =>_context.Produtos.Include(c => c.Categoria);

        public IEnumerable<Produtos> produtosEmDestaque => _context.Produtos.Where(p => p.Destaque).Include(c => c.Categoria);


        public Produtos GetProduto(int produtoId)
        {
   
            return _context.Produtos.FirstOrDefault(p => p.Id_Produto == produtoId);
        }
    }
}
