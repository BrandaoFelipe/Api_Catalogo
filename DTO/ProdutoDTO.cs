﻿using APICatalogo.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }
        [Required]
        [StringLength(80)]        
        public string? Nome { get; set; }

        [Required]
        [StringLength(300)]
        public string? Descricao { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(300)]
        public string? ImagemUrl { get; set; }    
        public int CategoriaId { get; set; }
    }
}
