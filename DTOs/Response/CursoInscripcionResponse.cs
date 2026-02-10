namespace KiwdyAPI.DTOs.Response
{
    public class CursoInscripcionResponse
    {
        public int IdCurso { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string PortadaUrl { get; set; }

        public decimal Precio { get; set; }
        public string Instructor { get; set; }
        public bool EstaInscripto { get; set; }
        public IList<SeccionResponse> Secciones { get; set; }
    }
}
