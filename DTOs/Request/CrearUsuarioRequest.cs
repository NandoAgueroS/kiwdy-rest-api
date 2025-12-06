using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Request
{
    public class CrearUsuarioRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100, ErrorMessage = "El apellido no puede tener más de 100 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [MaxLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        [MaxLength(20, ErrorMessage = "La clave no puede tener más de 20 caracteres")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public int? Rol { get; set; }
    }
}
