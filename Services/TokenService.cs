using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KiwdyAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace KiwdyAPI.Services
{
    public class TokenService
    {
        public static string CrearToken(IConfiguration configuration, Usuario usuario)
        {
            var secreto = configuration["TokenAuthentication:SecretKey"];
            if (string.IsNullOrEmpty(secreto))
                throw new Exception("Falta configurar TokenAuthentication:Secret");
            var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.IdUsuario.ToString()),
                new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                new Claim(ClaimTypes.Role, usuario.RolNombre),
            };

            var token = new JwtSecurityToken(
                issuer: configuration["TokenAuthentication:Issuer"],
                audience: configuration["TokenAuthentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credenciales
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
