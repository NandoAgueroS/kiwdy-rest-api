using KiwdyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KiwdyAPI.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
    }
}
