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
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar(CrearUsuarioRequest usuarioRequest)
        {
            if (usuarioRequest.Rol.Value == 1)
                return BadRequest("No se seleccionó un rol válido");
            var usuario = usuarioRequest.Adapt<Usuario>();
            string claveHasheada = HashService.HashClave(
                _configuration["salt"],
                usuarioRequest.Clave
            );
            usuario.Clave = claveHasheada;

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return StatusCode(201, usuario.Adapt<UsuarioResponse>());
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var usuario = await _context
                .Usuarios.Where(u => u.Email == loginRequest.Email && u.Rol == loginRequest.Rol)
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("No se encontro el usuario");

            if (
                !HashService.VerificarClave(
                    _configuration["salt"],
                    loginRequest.Clave,
                    usuario.Clave
                )
            )
                return BadRequest("La clave es incorrecta");

            string token = TokenService.CrearToken(_configuration, usuario);
            return Ok(token);
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> Perfil()
        {
            var idUsuario = int.Parse(User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                return BadRequest("Error de autenticación");

            var usuario = await _context
                .Usuarios.Where(u => u.IdUsuario == idUsuario)
                .ProjectToType<UsuarioResponse>()
                .SingleOrDefaultAsync();

            return Ok(usuario);
        }

        [HttpGet("token-valido")]
        public IActionResult tokenValido()
        {
            return Ok();
        }
    }
}
