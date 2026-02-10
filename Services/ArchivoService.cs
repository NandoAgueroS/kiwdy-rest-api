using KiwdyAPI.DTOs;

namespace KiwdyAPI.Services
{
    public class ArchivoService
    {
        public static ArchivoInfo guardarArchivoEstatico(
            IWebHostEnvironment webEnv,
            String prefijo,
            IFormFile archivo
        )
        {
            string wwwPath = webEnv.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = prefijo + "_" + Guid.NewGuid() + Path.GetExtension(archivo.FileName);
            string pathCompleto = Path.Combine(path, fileName);

            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }
            var url = Path.Combine("/Uploads", fileName);
            ArchivoInfo archivoInfo = new ArchivoInfo { Nombre = fileName, Url = url };
            return archivoInfo;
        }

        public static ArchivoInfo guardarArchivoMaterial(String prefijo, IFormFile archivo)
        {
            string path = "./Materiales";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = prefijo + "_" + Guid.NewGuid() + Path.GetExtension(archivo.FileName);
            string pathCompleto = Path.Combine(path, fileName);

            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }
            var url = Path.Combine("./Materiales", fileName);
            ArchivoInfo archivoInfo = new ArchivoInfo { Nombre = fileName, Url = url };
            return archivoInfo;
        }
    }
}
