using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
                page.PageColor(Colors.Green.Accent1);
                page.Background().Border(70).BorderColor(Colors.Green.Darken1);
                page.Margin(40);

                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(20);

                        col.Item()
                            .Text("Certificado de finalización")
                            .FontSize(35)
                            .Bold()
                            .AlignCenter();

                        col.Item().Text($"Se certifica que el alumno").FontSize(22).AlignCenter();

                        col.Item().Text(NombreAlumno).FontSize(25).Bold().AlignCenter();

                        col.Item()
                            .Text($"Ha completado satisfactoriamente el curso")
                            .FontSize(18)
                            .AlignCenter();

                        col.Item().Text(NombreCurso).FontSize(20).Bold().AlignCenter();

                        col.Item().Text($"El día: {Fecha:dd/MM/yyyy}").AlignCenter();
                    });
            });
        }
    }
}
