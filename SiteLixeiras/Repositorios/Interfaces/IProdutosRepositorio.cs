using SiteLixeiras.Models;

namespace SiteLixeiras.Repositorios.Interfaces
{
    public interface IProdutosRepositorio
    {
        IEnumerable<Produtos> produtos { get; }
        IEnumerable<Produtos> produtosEmDestaque { get; }
        Produtos GetProduto(int produtoId);
    }
}
