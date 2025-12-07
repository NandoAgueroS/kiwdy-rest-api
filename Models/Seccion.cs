using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.Models
{
    public class Seccion
    {
        [Key]
        public int IdSeccion { get; set; }

        public string Titulo { get; set; }

        public string Contenido { get; set; }

        public string? VideoUrl { get; set; }

        [ForeignKey("IdSeccion")]
        public IList<Material>? Materiales { get; set; }

        public int Orden { get; set; }

        [ForeignKey("IdCurso")]
        public int IdCurso { get; set; }

        public bool Eliminado { get; set; } = false;
    }
}
