using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace KiwdyAPI.Services
{
    public class CertificadoDocument : IDocument
    {
        public string NombreAlumno;
        public string NombreCurso;
        public DateTime Fecha;

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(20);

                        col.Item()
                            .Text("Certificado de finalización")
                            .FontSize(28)
                            .Bold()
                            .AlignCenter();

                        col.Item().Text($"Se certifica que el alumno").FontSize(16).AlignCenter();

                        col.Item().Text(NombreAlumno).FontSize(22).Bold().AlignCenter();

                        col.Item().Text($"Ha completado satisfactoriamente el curso").AlignCenter();

                        col.Item().Text(NombreCurso).FontSize(18).Bold().AlignCenter();

                        col.Item().Text($"El día: {Fecha:dd/MM/yyyy}").AlignCenter();
                    });
            });
        }
    }
}
