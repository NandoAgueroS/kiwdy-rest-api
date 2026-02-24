using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Request
{
    public class ActualizarHabilitadoCursoRequest
    {
        [Required]
        public bool? Habilitado { get; set; }
    }
}
