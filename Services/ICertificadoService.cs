namespace KiwdyAPI.Services
{
    public interface ICertificadoService
    {
        byte[] Generar(string nombreAlumno, string nombreCurso);
    }
}
