using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.Models
{
    public enum Roles
    {
        Administrador = 1,
        Instructor = 2,
        Alumno = 3,
    }

    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public string Clave { get; set; }

        [NotMapped]
        public string RolNombre { get; set; }

        public int Rol { get; set; }

        public bool Eliminado { get; set; } = false;

        public static IDictionary<int, string> GetRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipo = typeof(Roles);
            foreach (int valor in Enum.GetValues(tipo))
            {
                roles.Add((int)valor, Enum.GetName(tipo, valor) ?? "");
            }
            return roles;
        }
    }
}
