using KiwdyAPI.DTOs.Request;
using KiwdyAPI.DTOs.Response;
using KiwdyAPI.Filters;
using KiwdyAPI.Models;
using KiwdyAPI.Repositories;
using KiwdyAPI.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiwdyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(UsuarioExistenteFilter))]
    public class InscripcionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webEnv;

        public InscripcionesController(DataContext context, IWebHostEnvironment webEnv)
        {
            _context = context;
            _webEnv = webEnv;
        }

        [Authorize(Policy = "Alumno")]
        [HttpPost("{idCurso}")]
        public async Task<IActionResult> Crear(int idCurso)
        {
            var idUsuarioAlumno = (int?)HttpContext.Items["idUsuario"];
            if (idUsuarioAlumno == null || idUsuarioAlumno.Value == 0)
                return Unauthorized();
            var curso = await _context.Cursos.SingleOrDefaultAsync(c =>
                c.IdCurso == idCurso && c.Eliminado == false
            );
            if (curso == null)
                return BadRequest("El curso no existe");

            Inscripcion inscripcion = new Inscripcion
            {
                IdCurso = idCurso,
                Curso = curso,
                IdUsuarioAlumno = idUsuarioAlumno.Value,
                Estado = Inscripcion.EstadoInscripcion.Solicitada,
            };

            await _context.Inscripciones.AddAsync(inscripcion);
            await _context.SaveChangesAsync();

            return StatusCode(201, inscripcion.Adapt<InscripcionResponse>());
        }

        [Authorize(Policy = "Instructor")]
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] Inscripcion.EstadoInscripcion? estado)
        {
            var idUsuarioInstructor = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Inscripcion> inscripcionesQuery = _context.Inscripciones.Where(i =>
                i.Curso.IdUsuarioInstructor == idUsuarioInstructor.Value && i.Eliminado == false
            );
            if (estado != null)
            {
                inscripcionesQuery = inscripcionesQuery.Where(i => i.Estado == estado.Value);
            }
            var inscripciones = await inscripcionesQuery
                .ProjectToType<InscripcionResponse>()
                .ToListAsync();
            return Ok(inscripciones);
        }

        [Authorize(Policy = "Instructor")]
        [HttpPatch("{idInscripcion}/estado/{estado}")]
        public async Task<IActionResult> CambiarEstado(
            [FromRoute] int idInscripcion,
            [FromRoute] Inscripcion.EstadoInscripcion estado
        )
        {
            var idUsuarioInstructor = (int?)HttpContext.Items["idUsuario"];

            var inscripcion = await _context.Inscripciones.SingleOrDefaultAsync(i =>
                i.IdInscripcion == idInscripcion
                && i.Curso.IdUsuarioInstructor == idUsuarioInstructor.Value
                && i.Eliminado == false
            );
            if (inscripcion == null)
                return BadRequest("No se encontro la inscripcion");

            inscripcion.Estado = estado;

            await _context.SaveChangesAsync();

            return Ok(inscripcion.Adapt<InscripcionResponse>());
        }
    }
}
