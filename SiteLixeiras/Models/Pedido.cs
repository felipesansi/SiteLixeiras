using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SiteLixeiras.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }

        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser? Usuario { get; set; }

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

        public List<PedidoDetalhe> PedidoItens { get; set; } = new();

        public int EnderecoEntregaId { get; set; }

        [ForeignKey("EnderecoEntregaId")]
        public EnderecoEntrega? EnderecoEntrega { get; set; }
    }
}
