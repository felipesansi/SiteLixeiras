using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    [Table("CarrinhoCompraItens")]
    public class CarrinhoCompraItem
    {
        [Key]
        public int CarrinhoCompraItemId { get; set; } // Nome singular e padrão

        // Chave estrangeira para Produto
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produtos Produtos { get; set; } // Navegação para produto

        public int Quantidade { get; set; }

        [MaxLength(200)]
        public string CarrinhoCompraId { get; set; } // Identifica o carrinho por sessão
        public string UsuarioId { get; set; } // Identifica o usuário dono do carrinho
    }
}
