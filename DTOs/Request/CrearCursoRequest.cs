using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.DTOs.Request
{
    public class CrearCursoRequest
    {
        [Required(ErrorMessage = "El título del curso es requerido")]
        [MaxLength(100, ErrorMessage = "El título del curso no puede tener más de 100 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción del curso es requerida")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio del curso es requerido")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Precio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NotaAprobacion { get; set; }

        public IFormFile Portada { get; set; }
    }
}
