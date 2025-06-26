using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    [Table("Produtos")]
    public class Produtos
    {
        [Key]
        public int Id_Produto { get; set; }

        [Required(ErrorMessage = "O campo Código é obrigatório.")]
        [Display(Name = "Código do Produto")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Nome do Produto")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [Display(Name = "Descrição do Produto")]
        [StringLength(400, ErrorMessage = "A descrição deve ter no máximo 400 caracteres.")]
        public required string Descricao { get; set; }

        [Required(ErrorMessage = "O campo Altura é obrigatório.")]
        [Display(Name = "Altura (cm)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal altura { get; set; }

        [Required(ErrorMessage = "O campo Largura é obrigatório.")]
        [Display(Name = "Largura (cm)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal largura { get; set; }

        [Required(ErrorMessage = "O campo Profundidade é obrigatório.")]
        [Display(Name = "Profundidade (cm)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal profundidade { get; set; }

        [Required]
        [Display(Name = "Preço do imóvel")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        [Display(Name = "Imagem do Produto")]
        public string? Imagem { get; set; }

        [Display(Name = "Imagem do Produto")]
        public string? ImagemThumbUrl { get; set; }

        [Display(Name = "Aparecer na página inicial")]
        public bool Destaque { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }

        public List<Foto> Fotos { get; set; } = new();

        [Display(Name = "Este produto é um Kit?")]
        public bool EhKit { get; set; } = false;

        // Produtos que compõem este kit (caso EhKit = true)
        public List<ProdutoKitItem> ItensDoKit { get; set; } = new();
    }
}
