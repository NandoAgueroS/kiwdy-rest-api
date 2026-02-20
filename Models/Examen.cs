using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.Models
{
    public enum Modalidad
    {
        Virtual,
        Presencial,
    }

    public class Examen
    {
        [Key]
        public int IdExamen { get; set; }

        public DateTime FechaYHora { get; set; }

        public Modalidad Modalidad { get; set; }

        public string? Link { get; set; }

        public string? Direccion { get; set; }

        public int Nota { get; set; } = -1;

        public int IdInscripcion { get; set; }

        [ForeignKey("IdInscripcion")]
        public Inscripcion Inscripcion { get; set; }

        public bool Eliminado { get; set; } = false;
    }
}
