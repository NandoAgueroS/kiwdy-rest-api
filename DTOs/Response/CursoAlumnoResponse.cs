namespace KiwdyAPI.DTOs.Response
{
    public class CursoAlumnoResponse
    {
        public int IdCurso { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string PortadaUrl { get; set; }

        public decimal Precio { get; set; }

        public bool EstaInscripto { get; set; }
        public bool EstaFinalizado { get; set; }

        public IList<SeccionResponse> Secciones { get; set; }
    }
}
