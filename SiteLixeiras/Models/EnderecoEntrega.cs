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

        public required string Nome { get; set; }

        [Display(Name = "Sobrenome: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public required string SobreNome { get; set; }
        [Display(Name = " Endereço : ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public required string Endereco { get; set; }
        

        [Display(Name = " Estado: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public required string Estado { get; set; }
        [Display(Name = " Cidade: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]

        public required string Cidade { get; set; }
        [Display(Name = "Telefone:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(25)]
        [DataType(DataType.PhoneNumber)]
        public required string Telefone { get; set; }
        [Display(Name = " E-mail:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email inválido")]
        [StringLength(50)]
        public required string Email { get; set; }
       
        [Display(Name = " CEP: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(8)]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "CEP inválido")]

        public required string Cep { get; set; }

        [Display(Name = "CPF:")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", ErrorMessage = "CPF inválido (formato: 000.000.000-00)")]
        [StringLength(14)]
        public required string CPF { get; set; }



    }
}
