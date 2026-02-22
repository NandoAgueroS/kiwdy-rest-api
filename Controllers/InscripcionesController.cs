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
        private readonly ICertificadoService _certificadoService;

        public InscripcionesController(
            DataContext context,
            IWebHostEnvironment webEnv,
            ICertificadoService certificadoService
        )
        {
            _context = context;
            _webEnv = webEnv;
            _certificadoService = certificadoService;
        }

        [Authorize(Policy = "Alumno")]
        [HttpPost("curso/{idCurso}")]
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
            };
            if (curso.Precio == 0)
            {
                inscripcion.Estado = Inscripcion.EstadoInscripcion.EnCurso;
                inscripcion.FechaInicio = DateTime.UtcNow;
            }
            else
            {
                inscripcion.Estado = Inscripcion.EstadoInscripcion.Solicitada;
            }

            await _context.Inscripciones.AddAsync(inscripcion);
            await _context.SaveChangesAsync();

            return StatusCode(201, inscripcion.Adapt<InscripcionResponse>());
        }

        [Authorize(Policy = "Instructor")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] Inscripcion.EstadoInscripcion? estado,
            [FromQuery] int? idCurso
        )
        {
            var idUsuarioInstructor = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Inscripcion> inscripcionesQuery = _context
                .Inscripciones.Include(i => i.Curso)
                .Include(i => i.UsuarioAlumno)
                .Where(i =>
                    i.Curso.IdUsuarioInstructor == idUsuarioInstructor.Value && i.Eliminado == false
                );
            if (estado != null)
            {
                inscripcionesQuery = inscripcionesQuery.Where(i => i.Estado == estado.Value);
            }
            if (idCurso != null)
            {
                inscripcionesQuery = inscripcionesQuery.Where(i => i.IdCurso == idCurso);
            }
            var inscripciones = await inscripcionesQuery
                .ProjectToType<InscripcionResponse>()
                .ToListAsync();
            return Ok(inscripciones);
        }

        [Authorize(Policy = "Alumno")]
        [HttpGet("alumno")]
        public async Task<IActionResult> Listar([FromQuery] string? tituloCurso)
        {
            var idUsuarioAlumno = (int?)HttpContext.Items["idUsuario"];

            IQueryable<Inscripcion> inscripcionesQuery = _context
                .Inscripciones.Include(i => i.Curso)
                .Include(i => i.UsuarioAlumno)
                .Where(i => i.IdUsuarioAlumno == idUsuarioAlumno.Value && i.Eliminado == false);
            if (tituloCurso != null)
            {
                inscripcionesQuery = inscripcionesQuery.Where(i =>
                    i.Curso.Titulo.Contains(tituloCurso)
                );
            }
            var inscripciones = await inscripcionesQuery
                .ProjectToType<InscripcionResponse>()
                .ToListAsync();
            return Ok(inscripciones);
        }

        [Authorize(Policy = "Instructor")]
        [HttpGet("{idInscripcion}")]
        public async Task<IActionResult> Buscar([FromRoute] int idInscripcion)
        {
            var idUsuarioInstructor = (int?)HttpContext.Items["idUsuario"];

            var inscripcion = await _context
                .Inscripciones.Include(i => i.Curso)
                .ThenInclude(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Include(i => i.UsuarioAlumno)
                .Include(i => i.SeccionesCompletadas)
                .ThenInclude(s => s.Seccion)
                .Where(i =>
                    i.Curso.IdUsuarioInstructor == idUsuarioInstructor.Value
                    && i.Eliminado == false
                    && i.IdInscripcion == idInscripcion
                )
                .Select(i => new InscripcionResponse
                {
                    IdInscripcion = i.IdInscripcion,
                    IdUsuarioAlumno = i.IdUsuarioAlumno,
                    Curso = i.Curso.Adapt<CursoResponse>(),
                    FechaInicio = i.FechaInicio,
                    FechaFin = i.FechaFin,
                    UsuarioAlumno = i.UsuarioAlumno.Adapt<UsuarioResponse>(),
                    Estado = i.Estado.ToString(),
                    UltimaSeccionCompletada = i.SeccionesCompletadas.Max(s => s.Seccion.Orden),
                })
                .SingleOrDefaultAsync();
            if (inscripcion == null)
                return NotFound();
            return Ok(inscripcion);
        }

        [Authorize(Policy = "Alumno")]
        [HttpGet("curso/{idCurso}")]
        public async Task<IActionResult> BuscarPorCurso([FromRoute] int idCurso)
        {
            var idUsuarioAlumno = (int?)HttpContext.Items["idUsuario"];

            var inscripcion = await _context
                .Inscripciones.Include(i => i.Curso)
                .ThenInclude(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Include(i => i.Curso)
                .ThenInclude(c => c.UsuarioInstructor)
                .Include(i => i.UsuarioAlumno)
                .Include(i => i.SeccionesCompletadas)
                .ThenInclude(s => s.Seccion)
                .Where(i =>
                    i.IdUsuarioAlumno == idUsuarioAlumno.Value
                    && i.Eliminado == false
                    && i.IdCurso == idCurso
                )
                .Select(i => new InscripcionResponse
                {
                    IdInscripcion = i.IdInscripcion,
                    IdUsuarioAlumno = i.IdUsuarioAlumno,
                    Curso = i.Curso.Adapt<CursoResponse>(),
                    FechaInicio = i.FechaInicio,
                    FechaFin = i.FechaFin,
                    UsuarioAlumno = i.UsuarioAlumno.Adapt<UsuarioResponse>(),
                    Estado = i.Estado.ToString(),
                    UltimaSeccionCompletada = i.SeccionesCompletadas.Max(s => s.Seccion.Orden),
                })
                .SingleOrDefaultAsync();
            if (inscripcion == null)
                return NotFound();
            return Ok(inscripcion);
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
            switch (estado)
            {
                case Inscripcion.EstadoInscripcion.EnCurso:
                    inscripcion.FechaInicio = DateTime.UtcNow.Date;
                    break;
                case Inscripcion.EstadoInscripcion.Certificada:
                    inscripcion.FechaFin = DateTime.UtcNow.Date;
                    break;
            }

            await _context.SaveChangesAsync();

            return Ok(inscripcion.Adapt<InscripcionResponse>());
        }

        [HttpPost("{idInscripcion}/secciones")]
        public async Task<IActionResult> marcarSeccionCompletada(
            int idInscripcion,
            [FromBody] MarcarSeccionCompletadaRequest request
        )
        {
            var inscripcion = await _context
                .Inscripciones.Include(i => i.Curso)
                .ThenInclude(c => c.Secciones)
                .Include(i => i.UsuarioAlumno)
                .SingleOrDefaultAsync(i => i.IdInscripcion == idInscripcion && !i.Eliminado);
            if (inscripcion == null)
                return NotFound();
            var seccion = await _context.Secciones.SingleOrDefaultAsync(s =>
                s.IdSeccion == request.idSeccion && !s.Eliminado && s.IdCurso == inscripcion.IdCurso
            );
            if (seccion == null)
                return NotFound();

            if (inscripcion.SeccionesCompletadas == null)
                inscripcion.SeccionesCompletadas = new List<SeccionCompletada>();

            inscripcion.SeccionesCompletadas.Add(
                new SeccionCompletada
                {
                    IdInscripcion = idInscripcion,
                    IdSeccion = request.idSeccion,
                }
            );

            if (inscripcion.Curso.Secciones.MaxBy(s => s.Orden).IdSeccion == request.idSeccion)
                if (inscripcion.Curso.NotaAprobacion != -1)
                {
                    inscripcion.Estado = Inscripcion.EstadoInscripcion.PendienteCertificacion;
                }
                else
                {
                    inscripcion.Estado = Inscripcion.EstadoInscripcion.Certificada;
                    var alumno = inscripcion.UsuarioAlumno;
                    var curso = inscripcion.Curso;
                    var certificado = _certificadoService.Generar(
                        alumno.Nombre + " " + alumno.Apellido,
                        curso.Titulo
                    );
                    var ruta = "Uploads/Certificados";
                    var nombre = "certificado" + idInscripcion + ".pdf";
                    ArchivoService.guardarBytes(ruta, nombre, certificado);
                    inscripcion.Certificado = ruta + "/" + nombre;
                    inscripcion.FechaFin = DateTime.UtcNow.Date;
                }
            await _context.SaveChangesAsync();
            return Ok(inscripcion.Adapt<InscripcionResponse>());
        }

        [HttpGet("{idInscripcion}/certificado")]
        public async Task<IActionResult> obtenerCertificado(int idInscripcion)
        {
            var inscripcion = await _context.Inscripciones.SingleOrDefaultAsync(i =>
                i.IdInscripcion == idInscripcion
            );
            if (inscripcion == null)
                return NotFound();

            var ruta = inscripcion.Certificado;

            if (!System.IO.File.Exists(ruta))
                return NotFound();

            var stream = System.IO.File.OpenRead(ruta);

            return File(stream, "application/octet-stream", Path.GetFileName(ruta));
        }
    }
}
