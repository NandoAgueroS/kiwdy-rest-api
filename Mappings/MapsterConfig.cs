using KiwdyAPI.DTOs.Request;
using KiwdyAPI.DTOs.Response;
using KiwdyAPI.Models;
using Mapster;

namespace KiwdyAPI.Mappings
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CrearUsuarioRequest, Usuario>();

            config.NewConfig<Usuario, UsuarioResponse>();

            config.NewConfig<CrearCursoRequest, Curso>();

            config.NewConfig<Curso, CursoResponse>();
            config.NewConfig<Curso, CursoAlumnoResponse>();

            config.NewConfig<CrearSeccionRequest, Seccion>();

            config.NewConfig<Seccion, SeccionResponse>();
        }
    }
}
