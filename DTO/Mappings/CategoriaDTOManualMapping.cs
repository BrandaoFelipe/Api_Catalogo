using APICatalogo.Models;

namespace APICatalogo.DTO.Mappings
{
    public static class CategoriaDTOManualMapping
    {
        public static CategoriaDTO? ToCategoriaDTO(this Categoria categoria)
        {
            var categoriaDto = new CategoriaDTO()
            {
                CategoriaId = categoria.CategoriaId,
                Name = categoria.Name,
                ImagemUrl = categoria.ImagemUrl
            };
            return categoriaDto;
        }
        public static Categoria? ToCategoria(this CategoriaDTO categoriaDto)
        {
            var categoria = new Categoria()
            {
                CategoriaId = categoriaDto.CategoriaId,
                Name = categoriaDto.Name,
                ImagemUrl = categoriaDto.ImagemUrl
            };
            return categoria;
        }
        public static IEnumerable<CategoriaDTO>ToCategoriaDTOList(this IEnumerable<Categoria> categorias)
        {
            if(categorias is null || !categorias.Any())
            {
                return new List<CategoriaDTO>();
            }
            return categorias.Select(categoria => new CategoriaDTO
            {
                CategoriaId = categoria.CategoriaId,
                Name = categoria.Name,
                ImagemUrl = categoria.ImagemUrl
            }).ToList();
        }
    }
}
