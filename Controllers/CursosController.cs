using KiwdyAPI.DTOs.Request;
using KiwdyAPI.DTOs.Response;
using KiwdyAPI.Models;
using KiwdyAPI.Repositories;
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

            curso.IdUsuarioInstructor = idUsuario;

            await _context.Cursos.AddAsync(curso);

            await _context.SaveChangesAsync();

            return StatusCode(201, curso.Adapt<CursoResponse>());
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

        [HttpPost("secciones/{idSeccion}/video")]
        [RequestSizeLimit(500_000_000)]
        public async Task<IActionResult> SeccionVideo(IFormFile video, [FromRoute] int idSeccion)
        {
            var idUsuario = int.Parse(User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                return BadRequest("Error de autenticación");

            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u =>
                u.IdUsuario == idUsuario && u.Eliminado == false
            );

            if (usuario == null)
                return BadRequest("No se encontró el usuario");

            var seccion = await _context.Secciones.SingleOrDefaultAsync(s =>
                s.IdSeccion == idSeccion && s.Eliminado == false
            );

            string wwwPath = _webEnv.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = "seccion_video_" + Guid.NewGuid() + Path.GetExtension(video.FileName);
            string pathCompleto = Path.Combine(path, fileName);

            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            {
                video.CopyTo(stream);
            }

            string videoUrl = Path.Combine("/Uploads", fileName);

            seccion.VideoUrl = videoUrl;
            await _context.SaveChangesAsync();

            return StatusCode(201, videoUrl);
        }

        [HttpPost("secciones/{idSeccion}/material-extra")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> SeccionVideo(
            IList<IFormFile> materialExtra,
            [FromRoute] int idSeccion
        )
        {
            var idUsuario = int.Parse(User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                return BadRequest("Error de autenticación");

            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u =>
                u.IdUsuario == idUsuario && u.Eliminado == false
            );

            if (usuario == null)
                return BadRequest("No se encontró el usuario");

            var seccion = await _context.Secciones.SingleOrDefaultAsync(s =>
                s.IdSeccion == idSeccion && s.Eliminado == false
            );
            string wwwPath = _webEnv.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (seccion.Materiales == null)
                seccion.Materiales = new List<Material>();
            foreach (IFormFile material in materialExtra)
            {
                string fileName =
                    "seccion_material_" + Guid.NewGuid() + Path.GetExtension(material.FileName);
                string pathCompleto = Path.Combine(path, fileName);

                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    material.CopyTo(stream);
                }

                string materialUrl = Path.Combine("/Uploads", fileName);
                seccion.Materiales.Add(
                    new Material
                    {
                        Nombre = material.FileName,
                        Url = materialUrl,
                        IdSeccion = seccion.IdSeccion,
                    }
                );
            }

            await _context.SaveChangesAsync();

            return StatusCode(201, seccion.Materiales);
        }
    }
}
