using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    [Table("Foto")]
    public class Foto
    {
        [Key]
        public int FotoId { get; set; }
       
        [Required]
        [StringLength(200)] // Tamanho máximo da URL
        public string Url { get; set; }

        public int ProdutoId { get; set; } // Chave estrangeira para o produto
        [ForeignKey("ProdutoId")]
        public Produtos Produto { get; set; } // Navegação para o produto


    }
}
