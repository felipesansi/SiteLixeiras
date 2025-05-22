using SiteLixeiras.Models;

namespace SiteLixeiras.ViewModel
{
    public class UsuarioViewmodel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }

        public List<EnderecoEntrega> Entrega { get; set; } = new();
        public DateTime? DataCriacao { get; set; }
    }
}
