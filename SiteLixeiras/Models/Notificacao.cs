using System.ComponentModel.DataAnnotations;

namespace SiteLixeiras.Models
{
    public class Notificacao
    {

        [Key]
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public string Mensagem { get; set; }

        public bool Lida { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

    }
}
