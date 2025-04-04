using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key] 
        public int IdCategoria { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome da Categoria")]
        public required string Nome { get; set; }

        [Required]
        [StringLength(400, ErrorMessage = "A descrição deve ter no máximo 400 caracteres.")]
        [Display(Name = "Descrição da Categoria")]
        public required string Descricao { get; set; }

        public List<Produtos>? Produtos { get; set; } = new List<Produtos>();
    }
}
