using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    public class PedidoDetalhe
    {
        public int IdPedido { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

      
        public Pedido Pedido { get; set; }
        public Produtos Produto { get; set; }
    }
}
