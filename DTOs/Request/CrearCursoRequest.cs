using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Request
{
    public class CrearCursoRequest
    {
        [Required(ErrorMessage = "El título del curso es requerido")]
        [MaxLength(100, ErrorMessage = "El título del curso no puede tener más de 100 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción del curso es requerida")]
        [MaxLength(
            500,
            ErrorMessage = "La descripción del curso no puede tener más de 255 caracteres"
        )]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio del curso es requerido")]
        public decimal Precio { get; set; }

        public IFormFile Portada { get; set; }
    }
}
