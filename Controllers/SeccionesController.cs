using KiwdyAPI.DTOs;
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
    public class SeccionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webEnv;

        public SeccionesController(DataContext context, IWebHostEnvironment webEnv)
        {
            _context = context;
            _webEnv = webEnv;
        }

        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public async Task<IActionResult> Crear(CrearSeccionRequest seccionRequest)
        {
            var idUsuario = int.Parse(User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                return BadRequest("Error de autenticación");

            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u =>
                u.IdUsuario == idUsuario && u.Eliminado == false
            );

            if (usuario == null)
                return BadRequest("No se encontró el usuario");

            var seccion = seccionRequest.Adapt<Seccion>();
            var video = seccionRequest.Video;
            var materialExtra = seccionRequest.MaterialExtra;

            if (video != null)
            {
                seccion.VideoUrl = ArchivoService
                    .guardarArchivoEstatico(_webEnv, "seccion_video", video)
                    .Url;
            }
            if (materialExtra != null)
            {
                if (seccion.Materiales == null)
                    seccion.Materiales = new List<Material>();
                foreach (IFormFile material in materialExtra)
                {
                    ArchivoInfo archivoInfo = ArchivoService.guardarArchivoMaterial(
                        "seccion_material",
                        material
                    );
                    seccion.Materiales.Add(
                        new Material { Nombre = material.FileName, Url = archivoInfo.Url }
                    );
                }
            }

            await _context.Secciones.AddAsync(seccion);

            await _context.SaveChangesAsync();
            Console.WriteLine("saved");
            Console.WriteLine(seccion);

            // return StatusCode(201, seccion.Adapt<SeccionResponse>());
            return StatusCode(201, new { mensaje = "ok" });
        }

        [HttpGet("material/{idMaterial}")]
        [AllowAnonymous]
        public async Task<IActionResult> obtenerMaterial(int idMaterial)
        {
            var material = await _context.Materiales.SingleOrDefaultAsync(m =>
                m.IdMaterial == idMaterial
            );
            var ruta = material.Url;
            if (!System.IO.File.Exists(ruta))
                return NotFound();
            var stream = System.IO.File.OpenRead(ruta);
            return File(stream, "application/octet-stream", Path.GetFileName(ruta));
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
