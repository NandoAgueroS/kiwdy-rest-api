using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Request
{
    public class CrearSeccionRequest
    {
        public int IdCurso { get; set; }

        [Required(ErrorMessage = "El título de la seccion es requerido")]
        [MaxLength(
            100,
            ErrorMessage = "El título de la seccion no puede tener más de 100 caracteres"
        )]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El contenido de la seccion es requerido")]
        public string Contenido { get; set; }

        [Required(ErrorMessage = "El numero de orden de la seccion es requerido")]
        public int? Orden { get; set; }

        public IFormFile Video { get; set; }
        public IList<IFormFile>? MaterialExtra { get; set; }
    }
}
