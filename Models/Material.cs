using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiwdyAPI.Models
{
    public class Material
    {
        [Key]
        public int IdMaterial { get; set; }

        public string Nombre { get; set; }

        public string Url { get; set; }

        [ForeignKey("IdSeccion")]
        public int IdSeccion { get; set; }
    }
}
