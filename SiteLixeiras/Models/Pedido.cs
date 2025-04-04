using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteLixeiras.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }

        [Display(Name = " Nome: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]

        public required string Nome { get; set; }

        [Display(Name = "Sobrenome: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public required string SobreNome { get; set; }
        [Display(Name = " Endereço 1: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public required string Endereco1 { get; set; }
        [Display(Name = "Endereço 2: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public  required string Endereco2 { get; set; }
        [Display(Name = " Estado: ")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(999)]
        public  required string Estado { get; set; }
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

        [Column(TypeName = "decimal(18,2)")]
        [ScaffoldColumn(false)]
        [Display(Name = "Total do Pedido")]
        public decimal PedidoTotal { get; set; }

        [Display(Name = "Total itens pedido")]
        [ScaffoldColumn(false)]

        public int TotalItensPedidos { get; set; }
        public DateTime PedidoEnviado { get; set; }
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]

        public DateTime? PedididoEntregue { get; set; }
        public List<PedidoDetalhe> PedidoItens { get; set; }
    }
}
