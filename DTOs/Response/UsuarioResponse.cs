using System.ComponentModel.DataAnnotations;

namespace KiwdyAPI.DTOs.Response
{
    public class UsuarioResponse
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public int? Rol { get; set; }
    }
}
