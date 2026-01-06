using KiwdyAPI.DTOs.Request;
using KiwdyAPI.DTOs.Response;
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
    public class CursosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webEnv;

        public CursosController(DataContext context, IWebHostEnvironment webEnv)
        {
            _context = context;
            _webEnv = webEnv;
        }

        public async Task<IActionResult> Crear(CrearCursoRequest cursoRequest)
        {
            var idUsuario = int.Parse(User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                return BadRequest("Error de autenticación");

            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u =>
                u.IdUsuario == idUsuario && u.Eliminado == false
            );

            if (usuario == null)
                return BadRequest("No se encontró el usuario");

            var curso = cursoRequest.Adapt<Curso>();
            var portada = cursoRequest.Portada;

            curso.IdUsuarioInstructor = idUsuario;

            curso.PortadaUrl = ArchivoService.guardarArchivo(_webEnv, "curso_imagen", portada).Url;

            await _context.Cursos.AddAsync(curso);

            await _context.SaveChangesAsync();

            return StatusCode(201, curso.Adapt<CursoResponse>());
        }

        [HttpGet("{idCurso}")]
        public async Task<IActionResult> Buscar([FromRoute] int idCurso)
        {
            var curso = await _context
                .Cursos.Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
                .SingleOrDefaultAsync(c => c.IdCurso == idCurso && c.Eliminado == false);
            return Ok(curso);
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromRoute] int idCurso)
        {
            var cursos = await _context
                .Cursos.Include(c => c.Secciones)
                .ThenInclude(s => s.Materiales)
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
