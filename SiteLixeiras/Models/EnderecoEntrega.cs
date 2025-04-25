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


        [Display(Name = " Nome: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]

        public  string? Nome { get; set; }

        [Display(Name = "Sobrenome: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public string? SobreNome { get; set; }
        [Display(Name = " Rua : ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public  string Rua { get; set; }
        [Display(Name = " Bairro : ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public string ? Bairro { get; set; }


        [Display(Name = " Estado: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public  string? Estado { get; set; }
        [Display(Name = " Cidade: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]

        public  string ?Cidade { get; set; }
        [Display(Name = "Telefone:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(25)]
        [DataType(DataType.PhoneNumber)]
        public string? Telefone { get; set; }
        
        [Display(Name = " CEP: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(8)]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido")]
        public  string? Cep { get; set; }

        [Display(Name = "CPF:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", ErrorMessage = "CPF inválido (formato: 000.000.000-00)")]
        [StringLength(14)]
        public  string? CPF { get; set; }
      
        [Display(Name = "Número:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(10)]
        public string? Numero { get; set; }
        [Display(Name = "Complemento:")]
        [StringLength(999)]
        public string? Complemento { get; set; }

    }
}
