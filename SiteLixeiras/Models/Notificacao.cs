using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    public class Notificacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string Mensagem { get; set; }

        public bool EnviadaPeloAdmin { get; set; } = false;

        public string? Resposta { get; set; }

        public DateTime? DataResposta { get; set; }  

        public bool Lida { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }
    }
}
