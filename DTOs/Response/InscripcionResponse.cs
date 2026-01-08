namespace KiwdyAPI.DTOs.Response
{
    public class InscripcionResponse
    {
        public int IdInscripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public String Estado { get; set; }
        public CursoResponse Curso { get; set; }
        public int IdUsuarioAlumno { get; set; }
    }
}
