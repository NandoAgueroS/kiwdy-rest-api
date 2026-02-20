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
                .Where(c => c.IdCurso == idCurso && c.Eliminado == false)
                .Select(c => new CursoAlumnoResponse
                {
                    IdCurso = c.IdCurso,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    PortadaUrl = c.PortadaUrl,
                    EstaInscripto = c.Inscripciones.Any(i => i.IdUsuarioAlumno == idUsuario),
                    EstaFinalizado = c.Inscripciones.Any(i =>
                        i.IdUsuarioAlumno == idUsuario
                        && i.Estado == Inscripcion.EstadoInscripcion.Certificada
                    ),
                    Secciones = c.Secciones.OrderBy(s => s.Orden).Adapt<List<SeccionResponse>>(),
                })
                .FirstOrDefaultAsync();
            return Ok(curso);
        }

        // [HttpGet("{idCurso}/inscripcion-detalle")]
        // public async Task<IActionResult> BuscarInscripcionDetalle([FromRoute] int idCurso)
        // {
        //     var idUsuario = (int?)HttpContext.Items["idUsuario"];
        //     var curso = await _context
        //         .Cursos.Where(c => c.IdCurso == idCurso && !c.Eliminado)
        //         .Select(c => new CursoInscripcionResponse
        //         {
        //             IdCurso = c.IdCurso,
        //             Titulo = c.Titulo,
        //             Descripcion = c.Descripcion,
        //             PortadaUrl = c.PortadaUrl,
        //             Instructor = c.UsuarioInstructor.Nombre + " " + c.UsuarioInstructor.Apellido,
        //             Secciones = c.Secciones.Adapt<List<SeccionResponse>>(),
        //             EstaInscripto = c.Inscripciones.Any(i => i.IdUsuarioAlumno == idUsuario),
        //             IdInscripcion = c
        //                 .Inscripciones.Where(i => i.IdUsuarioAlumno == idUsuario)
        //                 .Select(i => i.IdInscripcion)
        //                 .FirstOrDefault(),
        //             UltimaSeccionCompletada = c
        //                 .Inscripciones.Where(i => i.IdUsuarioAlumno == idUsuario)
        //                 .Select(i => i.SeccionesCompletadas.Max(s => s.Seccion.Orden))
        //                 .FirstOrDefault(),
        //             EstadoInscripcion = c
        //                 .Inscripciones.Where(i => i.IdUsuarioAlumno == idUsuario)
        //                 .Select(i => i.Estado)
        //                 .FirstOrDefault()
        //                 .ToString(),
        //         })
        //         .FirstOrDefaultAsync();
        //     return Ok(curso);
        // }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var cursos = _context
                .Cursos.Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Where(c => c.Eliminado == false);
            if (User.IsInRole("Instructor"))
            {
                var idUsuario = (int?)HttpContext.Items["idUsuario"];
                cursos = cursos.Where(c => c.IdUsuarioInstructor == idUsuario);
            }
            var cursosList = await cursos
                .OrderByDescending(c => c.IdCurso)
                .ProjectToType<CursoResponse>()
                .ToListAsync();
            return Ok(cursosList);
        }

        [HttpGet("listar/populares")]
        public async Task<IActionResult> ListarPopulares()
        {
            var cursos = await _context
                .Cursos.Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .Where(c => !c.Eliminado)
                .OrderByDescending(c => _context.Inscripciones.Count(i => i.IdCurso == c.IdCurso))
                .ProjectToType<CursoResponse>()
                .ToListAsync();
            return Ok(cursos);
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
