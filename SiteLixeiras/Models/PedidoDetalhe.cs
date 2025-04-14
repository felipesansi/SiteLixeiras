using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteLixeiras.Models
{
    public class PedidoDetalhe
    {
        [Key]
        public int PedidoDetalheId { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Preco { get; set; }

        
        public virtual Pedido Pedido { get; set; }
        public virtual Produtos Produto { get; set; }
    }
}
