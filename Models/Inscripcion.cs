using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.Models
{
    public class Inscripcion
    {
        [Key]
        public int IdInscripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public EstadoInscripcion Estado { get; set; }

        public int IdCurso { get; set; }

        [ForeignKey("IdCurso")]
        public Curso Curso { get; set; }

        public int IdUsuarioAlumno { get; set; }

        [ForeignKey("IdUsuarioAlumno")]
        public Usuario? UsuarioAlumno { get; set; }
        public bool Eliminado { get; set; } = false;

        public List<SeccionCompletada> SeccionesCompletadas { get; set; }

        public enum EstadoInscripcion
        {
            Solicitada,
            EnCurso,
            PendienteCertificacion,
            Certificada,
        }
    }
}
