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
    public class ExamenesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ICertificadoService _certificadoService;

        public ExamenesController(DataContext context, ICertificadoService certificadoService)
        {
            _context = context;
            _certificadoService = certificadoService;
        }

        [Authorize(Policy = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Crear(CrearExamenRequest request)
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            var inscripcion = await _context
                .Inscripciones.Include(i => i.Curso)
                .SingleOrDefaultAsync(i =>
                    i.IdInscripcion == request.IdInscripcion && i.Eliminado == false
                );
            if (inscripcion == null)
                return BadRequest("La inscripcion no existe");
            if (inscripcion.Curso.IdUsuarioInstructor != idUsuario)
                return Forbid();

            var examen = request.Adapt<Examen>();
            await _context.Examenes.AddAsync(examen);
            await _context.SaveChangesAsync();

            return StatusCode(201, examen.Adapt<ExamenResponse>());
        }

        [Authorize(Policy = "InstructorOAlumno")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] bool? rendido,
            [FromQuery] DateTime? posterioresA,
            [FromQuery] int? idCurso
        )
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Examen> examenesQuery = _context
                .Examenes.Include(e => e.Inscripcion)
                .ThenInclude(i => i.UsuarioAlumno)
                .Include(e => e.Inscripcion)
                .ThenInclude(i => i.Curso)
                .ThenInclude(c => c.UsuarioInstructor)
                .Where(e => e.Eliminado == false);
            if (rendido != null && posterioresA != null)
            {
                return BadRequest("No se pueden combinar los filtros rendido y posterioresA");
            }
            if (rendido != null)
            {
                examenesQuery = examenesQuery.Where(e => (e.Nota == 0) == rendido.Value);
            }
            if (posterioresA != null)
            {
                examenesQuery = examenesQuery.Where(e => e.FechaYHora >= posterioresA.Value);
            }
            if (idCurso != null)
            {
                examenesQuery = examenesQuery.Where(e => e.Inscripcion.IdCurso == idCurso.Value);
            }

            if (User.IsInRole("Instructor"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.Curso.IdUsuarioInstructor == idUsuario.Value
                );
            else if (User.IsInRole("Alumno"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.IdUsuarioAlumno == idUsuario
                );
            var examenes = await examenesQuery.ProjectToType<ExamenResponse>().ToListAsync();
            return Ok(examenes);
        }

        [Authorize(Policy = "InstructorOAlumno")]
        [HttpGet("proximos")]
        public async Task<IActionResult> ListarProximos()
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Examen> examenesQuery = _context
                .Examenes.Include(e => e.Inscripcion)
                .ThenInclude(i => i.UsuarioAlumno)
                .Include(e => e.Inscripcion)
                .ThenInclude(i => i.Curso)
                .ThenInclude(c => c.UsuarioInstructor)
                .Where(e => e.Eliminado == false);
            if (User.IsInRole("Instructor"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.Curso.IdUsuarioInstructor == idUsuario.Value
                );
            else if (User.IsInRole("Alumno"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.IdUsuarioAlumno == idUsuario
                );
            var examenes = await examenesQuery.ProjectToType<ExamenResponse>().ToListAsync();
            return Ok(examenes);
        }

        [Authorize(Policy = "InstructorOAlumno")]
        [HttpGet("{idExamen}")]
        public async Task<IActionResult> Buscar([FromRoute] int idExamen)
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Examen> examenesQuery = _context
                .Examenes.Include(e => e.Inscripcion)
                .ThenInclude(i => i.UsuarioAlumno)
                .Include(e => e.Inscripcion)
                .ThenInclude(i => i.Curso)
                .ThenInclude(c => c.UsuarioInstructor)
                .Where(e => e.IdExamen == idExamen && e.Eliminado == false);

            if (User.IsInRole("Instructor"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.Curso.IdUsuarioInstructor == idUsuario.Value
                );
            else if (User.IsInRole("Alumno"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.IdUsuarioAlumno == idUsuario
                );

            var examen = await examenesQuery.ProjectToType<ExamenResponse>().SingleOrDefaultAsync();

            if (examen == null)
                return NotFound();
            return Ok(examen);
        }

        [Authorize(Policy = "InstructorOAlumno")]
        [HttpGet("inscripcion/{idInscripcion}")]
        public async Task<IActionResult> BuscarProximoPorInscripcion([FromRoute] int idInscripcion)
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Examen> examenesQuery = _context
                .Examenes.Include(e => e.Inscripcion)
                .ThenInclude(i => i.UsuarioAlumno)
                .Include(e => e.Inscripcion)
                .ThenInclude(i => i.Curso)
                .ThenInclude(c => c.UsuarioInstructor)
                .Where(e => e.IdInscripcion == idInscripcion && e.Eliminado == false);

            if (User.IsInRole("Instructor"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.Curso.IdUsuarioInstructor == idUsuario.Value
                );
            else if (User.IsInRole("Alumno"))
                examenesQuery = examenesQuery.Where(e =>
                    e.Inscripcion.IdUsuarioAlumno == idUsuario
                );

            var examen = await examenesQuery.ProjectToType<ExamenResponse>().ToListAsync();

            if (examen == null)
                return NotFound();
            return Ok(examen);
        }

        [Authorize(Policy = "Instructor")]
        [HttpPatch("{idExamen}")]
        public async Task<IActionResult> CargarNota(
            [FromRoute] int idExamen,
            [FromBody] CargarNotaRequest request
        )
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            var examen = await _context
                .Examenes.Include(e => e.Inscripcion)
                .ThenInclude(i => i.UsuarioAlumno)
                .Include(e => e.Inscripcion)
                .ThenInclude(i => i.Curso)
                .ThenInclude(c => c.UsuarioInstructor)
                .Where(e =>
                    e.IdExamen == idExamen
                    && e.Eliminado == false
                    && e.Inscripcion.Curso.IdUsuarioInstructor == idUsuario.Value
                )
                .SingleOrDefaultAsync();

            if (examen == null)
                return NotFound();
            if (examen.Inscripcion.Curso.NotaAprobacion <= request.Nota)
            {
                examen.Inscripcion.Estado = Inscripcion.EstadoInscripcion.Certificada;
                examen.Inscripcion.FechaFin = DateTime.UtcNow.Date;
                var alumno = examen.Inscripcion.UsuarioAlumno;
                var curso = examen.Inscripcion.Curso;
                var idInscripcion = examen.IdInscripcion;
                var certificado = _certificadoService.Generar(
                    alumno.Nombre + " " + alumno.Apellido,
                    curso.Titulo
                );
                var ruta = "Uploads/Certificados";
                var nombre = "certificado" + idInscripcion + ".pdf";
                ArchivoService.guardarBytes(ruta, nombre, certificado);
                examen.Inscripcion.Certificado = Path.Combine(ruta, nombre);
            }

            examen.Nota = request.Nota;
            await _context.SaveChangesAsync();

            return Ok(examen.Adapt<ExamenResponse>());
        }
    }
}
