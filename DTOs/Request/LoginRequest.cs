using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public int? Rol { get; set; }
    }
}
