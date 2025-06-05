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
        [StringLength(300)]
        public string Url { get; set; } // <- Link visível (https://...raw=1)

        [Required]
        [StringLength(300)]
        public string CaminhoDropbox { get; set; } // <- Caminho real no Dropbox (/LixeirasIcena/...)

        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produtos Produto { get; set; }
    }
}
