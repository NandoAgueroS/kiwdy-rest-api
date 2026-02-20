using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Request
{
    public class CargarNotaRequest
    {
        [Required(ErrorMessage = "La nota es requerida")]
        public int Nota { get; set; }
    }
}
