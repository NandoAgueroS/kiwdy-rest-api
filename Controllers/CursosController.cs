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
    public class CursosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webEnv;

        public CursosController(DataContext context, IWebHostEnvironment webEnv)
        {
            _context = context;
            _webEnv = webEnv;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearCursoRequest cursoRequest)
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            var curso = cursoRequest.Adapt<Curso>();
            var portada = cursoRequest.Portada;

            curso.IdUsuarioInstructor = idUsuario.Value;

            curso.PortadaUrl = ArchivoService
                .guardarArchivoEstatico(_webEnv, "curso_imagen", portada)
                .Url;

            await _context.Cursos.AddAsync(curso);

            await _context.SaveChangesAsync();

            return StatusCode(201, curso.Adapt<CursoResponse>());
        }

        [HttpGet("{idCurso}")]
        public async Task<IActionResult> Buscar([FromRoute] int idCurso)
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];
            var curso = await _context
                .Cursos.Include(c => c.Inscripciones)
                .Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Include(c => c.UsuarioInstructor)
                .Where(c => c.IdCurso == idCurso && c.Eliminado == false)
                .Select(c => new CursoAlumnoResponse
                {
                    IdCurso = c.IdCurso,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    PortadaUrl = c.PortadaUrl,
                    Habilitado = c.Habilitado,
                    Precio = c.Precio,
                    NotaAprobacion = c.NotaAprobacion.Value,
                    EstaInscripto = c.Inscripciones.Any(i =>
                        i.IdUsuarioAlumno == idUsuario
                        && i.Estado != Inscripcion.EstadoInscripcion.Solicitada
                    ),
                    EstaFinalizado = c.Inscripciones.Any(i =>
                        i.IdUsuarioAlumno == idUsuario
                        && i.Estado == Inscripcion.EstadoInscripcion.Certificada
                    ),
                    Secciones = c.Secciones.OrderBy(s => s.Orden).Adapt<List<SeccionResponse>>(),
                })
                .FirstOrDefaultAsync();
            return Ok(curso);
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar([FromQuery] string? tituloCurso)
        {
            var cursosQuery = _context
                .Cursos.Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Include(c => c.UsuarioInstructor)
                .Where(c => c.Eliminado == false);
            if (User.IsInRole("Instructor"))
            {
                var idUsuario = (int?)HttpContext.Items["idUsuario"];
                cursosQuery = cursosQuery.Where(c => c.IdUsuarioInstructor == idUsuario);
            }
            if (User.IsInRole("Alumno"))
            {
                cursosQuery = cursosQuery.Where(c => c.Habilitado);
            }
            if (tituloCurso != null)
            {
                cursosQuery = cursosQuery.Where(c => c.Titulo.Contains(tituloCurso));
            }
            var cursosList = await cursosQuery
                .OrderByDescending(c => c.IdCurso)
                .ProjectToType<CursoResponse>()
                .ToListAsync();
            return Ok(cursosList);
        }

        [HttpGet("listar/populares")]
        public async Task<IActionResult> ListarPopulares()
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            var cursosQuery = _context
                .Cursos.Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Include(c => c.UsuarioInstructor)
                .Where(c => !c.Eliminado);
            if (User.IsInRole("Instructor"))
            {
                cursosQuery = cursosQuery.Where(c => c.IdUsuarioInstructor == idUsuario);
            }
            if (User.IsInRole("Alumno"))
            {
                cursosQuery = cursosQuery.Where(c => c.Habilitado);
            }
            var cursos = await cursosQuery
                .OrderByDescending(c => _context.Inscripciones.Count(i => i.IdCurso == c.IdCurso))
                .ProjectToType<CursoResponse>()
                .ToListAsync();
            return Ok(cursos);
        }

        [HttpPatch("habilitado/{idCurso}")]
        public async Task<IActionResult> actualizarHabilitado(
            int idCurso,
            [FromBody] ActualizarHabilitadoCursoRequest request
        )
        {
            var idUsuario = (int?)HttpContext.Items["idUsuario"];

            var curso = await _context.Cursos.SingleOrDefaultAsync(c =>
                c.IdCurso == idCurso && c.IdUsuarioInstructor == idUsuario && !c.Eliminado
            );

            if (curso == null)
                return NotFound();

            curso.Habilitado = request.Habilitado.Value;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{idCurso}/portada")]
        public async Task<IActionResult> Portada(IFormFile portada, [FromRoute] int idCurso)
        {
            var idUsuario = int.Parse(User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                return BadRequest("Error de autenticación");

            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u =>
                u.IdUsuario == idUsuario && u.Eliminado == false
            );

            if (usuario == null)
                return BadRequest("No se encontró el usuario");

            var curso = await _context.Cursos.SingleOrDefaultAsync(c =>
                c.IdCurso == idCurso && c.Eliminado == false
            );

            string wwwPath = _webEnv.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName =
                "curso_imagen_" + Guid.NewGuid() + Path.GetExtension(portada.FileName);
            string pathCompleto = Path.Combine(path, fileName);

            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            {
                portada.CopyTo(stream);
            }

            string portadaUrl = Path.Combine("/Uploads", fileName);

            curso.PortadaUrl = portadaUrl;
            await _context.SaveChangesAsync();

            return StatusCode(201, portadaUrl);
        }
    }
}
