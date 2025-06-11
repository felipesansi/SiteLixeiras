using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SiteLixeiras.Models
{
    [Table("EnderecoEntrega")]
    public class EnderecoEntrega
    {
        [Key]
        public int EnderecoEntregaId { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser? User { get; set; }

        [Display(Name = "Nome:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string Nome { get; set; }

        [Display(Name = "Sobrenome:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string SobreNome { get; set; }

        [Display(Name = "Rua:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string Rua { get; set; }

        [Display(Name = "Bairro:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string Bairro { get; set; }

        [Display(Name = "Estado:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string Estado { get; set; }

        [Display(Name = "Cidade:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string Cidade { get; set; }

        [Display(Name = "Telefone:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }

        [Display(Name = "CEP:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        [DataType(DataType.PostalCode)]
        public string Cep { get; set; }

        [Display(Name = "CPF:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string CPF { get; set; }

        [Display(Name = "Número:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(256)]
        [MaxLength(256)]
        public string Numero { get; set; }

        [Display(Name = "Complemento:")]
        [StringLength(512)]
        [MaxLength(512)]
        public string? Complemento { get; set; }

        
        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
