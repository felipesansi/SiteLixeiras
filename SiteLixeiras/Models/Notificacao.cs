using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        [Required]
        public string Mensagem { get; set; }

        public bool EnviadaPeloAdmin { get; set; } = false;

        public bool Lida { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

    
        public int? NotificacaoPaiId { get; set; }
        public Notificacao NotificacaoPai { get; set; }

        public ICollection<Notificacao> Respostas { get; set; } = new List<Notificacao>();
    }
}
