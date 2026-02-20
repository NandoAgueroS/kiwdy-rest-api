using QuestPDF.Fluent;

namespace KiwdyAPI.Services
{
    public class CertificadoService : ICertificadoService
    {
        public byte[] Generar(string nombreAlumno, string nombreCurso)
        {
            var document = new CertificadoDocument
            {
                NombreAlumno = nombreAlumno,
                NombreCurso = nombreCurso,
                Fecha = DateTime.UtcNow,
            };

            return document.GeneratePdf();
        }
    }
}
