using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.Models
{
    public class Curso
    {
        [Key]
        public int IdCurso { get; set; }

        [ForeignKey("IdUsuarioInstructor")]
        public int IdUsuarioInstructor { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string? PortadaUrl { get; set; }

        public decimal Precio { get; set; }

        public bool Habilitado { get; set; } = true;

        public bool Eliminado { get; set; } = false;

        [ForeignKey("IdCurso")]
        public IList<Seccion> Secciones { get; set; }
    }
}
