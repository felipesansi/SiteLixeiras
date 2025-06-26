using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    [Table("ProdutoKitItens")]
    public class ProdutoKitItem
    {
        [Key]
        public int Id { get; set; }

        public int ProdutoKitId { get; set; }

        [ForeignKey("ProdutoKitId")]
        public Produtos? ProdutoKit { get; set; }

        public int ProdutoFilhoId { get; set; }

        [ForeignKey("ProdutoFilhoId")]
        public Produtos? ProdutoFilho { get; set; }

        [Range(1, 999)]
        public int Quantidade { get; set; } = 1;
    }
}
