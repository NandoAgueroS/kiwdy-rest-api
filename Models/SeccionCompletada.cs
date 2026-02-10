namespace KiwdyAPI.Models
{
    public class SeccionCompletada
    {
        public int IdInscripcion { get; set; }
        public Inscripcion? Inscripcion { get; set; }
        public int IdSeccion { get; set; }
        public Seccion? Seccion { get; set; }
    }
}
