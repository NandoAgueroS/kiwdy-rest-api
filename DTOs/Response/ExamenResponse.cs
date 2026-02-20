using KiwdyAPI.Models;

namespace KiwdyAPI.DTOs.Response
{
    public class ExamenResponse
    {
        public int IdExamen { get; set; }

        public DateTime FechaYHora { get; set; }

        public Modalidad Modalidad { get; set; }

        public string Link { get; set; }

        public string Direccion { get; set; }

        public int Nota { get; set; }

        public InscripcionResponse Inscripcion { get; set; }
    }
}
