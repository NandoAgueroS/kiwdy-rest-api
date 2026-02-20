using System.ComponentModel.DataAnnotations;
using KiwdyAPI.Models;
using KiwdyAPI.Validations;

namespace KiwdyAPI.DTOs.Request
{
    public class CrearExamenRequest
    {
        [Required(ErrorMessage = "La modalidad es requerida")]
        public int? Modalidad { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [FechaFuturaValidation]
        public DateTime? FechaYHora { get; set; }

        public string? Link { get; set; }

        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El id de la inscripcion es requerido")]
        public int? IdInscripcion { get; set; }
    }
}
