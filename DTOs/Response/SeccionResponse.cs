using KiwdyAPI.Models;

namespace KiwdyAPI.DTOs.Response
{
    public class SeccionResponse
    {
        public int IdSeccion { get; set; }
        public int IdCurso { get; set; }

        public string Titulo { get; set; }

        public string Contenido { get; set; }

        public string? VideoUrl { get; set; }

        public int Orden { get; set; }

        public IList<Material> Materiales { get; set; }
    }
}
