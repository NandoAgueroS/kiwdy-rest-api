using KiwdyAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace KiwdyAPI.Filters
{
    public class UsuarioExistenteFilter : IAsyncActionFilter
    {
        private readonly DataContext _context;

        public UsuarioExistenteFilter(DataContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var idUsuario = int.Parse(context.HttpContext.User?.Identity?.Name ?? "0");

            if (idUsuario == 0)
                context.Result = new UnauthorizedObjectResult("Error de autenticación");

            var usuario = await _context.Usuarios.AnyAsync(u =>
                u.IdUsuario == idUsuario && u.Eliminado == false
            );

            if (!usuario)
                context.Result = new UnauthorizedObjectResult("Error de autenticación");

            context.HttpContext.Items.Add("idUsuario", idUsuario);

            await next();
        }
    }
}
