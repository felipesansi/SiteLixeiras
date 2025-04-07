using SiteLixeiras.Models;

namespace SiteLixeiras.Repositorios.Interfaces
{
    public interface ICategoriaRepositorio
    {
        IEnumerable<Categoria> categorias { get; }
    }
}
