using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KiwdyAPI.Models
{
    public class InscripcionRequest
    {
        public int IdCurso { get; set; }
    }
}
