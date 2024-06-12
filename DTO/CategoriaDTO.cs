using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO
{
    public class CategoriaDTO
    {
        public int CategoriaId { get; set; }

        [Required]
        [StringLength(60)]
        public string? Name { get; set; }

        [Required]
        [StringLength(300)]
        public string? ImagemUrl { get; set; }
    }
}
