using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Models
{
    public class LivrosIndexViewModel
    {
        [Display(Name = "Código")]
        public int? IdLivro { get; set; }

        [Display(Name = "Livro")]
        public string Nome { get; set; }
        public string Autor { get; set; }
        public int Ano { get; set; }

        [Display(Name = "Locatário")]
        public string UserId { get; set; }
        public string UserIdLogado { get; set; }
        public IEnumerable<Livro> Livros { get; set; }
    }
}
