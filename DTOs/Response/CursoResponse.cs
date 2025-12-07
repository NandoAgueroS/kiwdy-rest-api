namespace KiwdyAPI.DTOs.Response
{
    public class CursoResponse
    {
        public int IdCurso { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string PortadaUrl { get; set; }

        public decimal Precio { get; set; }

        public bool Habilitado { get; set; } = true;

        public bool Eliminado { get; set; } = false;

        public IList<SeccionResponse> Secciones { get; set; }
    }
}
