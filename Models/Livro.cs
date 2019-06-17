using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Livro
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo requerido.")]
        [MaxLength(100)]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo requerido.")]
        [MaxLength(100)]
        public string Autor { get; set; }
        public int Ano { get; set; }

        [Display(Name = "Locatário")]

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }


    }
}