using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO
{
    public class ProdutoDTOResponseRequest
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        decimal Preco { get; set; }
        public string? ImagemUrl { get; set; }
        public int CategoriaId { get; set; }
    }
}
