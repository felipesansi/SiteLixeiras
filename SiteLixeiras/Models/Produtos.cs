using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteLixeiras.Models
{
    [Table("Produtos")]
    public class Produtos
    {
        [Key]
        public int Id_Produto { get; set; }
        
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Código do Produto")]
        public int Codigo { get; set; }
      
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Nome do Produto")]
        public required string Nome { get; set; }
      
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Descrição do Produto")]
        [StringLength(400, ErrorMessage = "A descrição deve ter no máximo 400 caracteres.")]
        public required string Descricao { get; set; }
      
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Preço do Produto")]
        [Range(0.01, 9999.99, ErrorMessage = "O preço deve ser entre 0,01 e 9999,99.")]
        [Column(TypeName = "decimal(18,2)")]

        public decimal Preco { get; set; }

        [Display(Name = "Imagem do Produto")]
        public string? Imagem { get; set; }
        [Display (Name = "Aparecer na pagina inicial")]
        public bool Destaque {  get; set; }


    }
}
