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
        public DbSet<SeccionCompletada> SeccionesCompletadas { get; set; }
        public DbSet<Examen> Examenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<SeccionCompletada>()
                .HasKey(i => new { i.IdInscripcion, i.IdSeccion });
            modelBuilder
                .Entity<SeccionCompletada>()
                .HasOne(x => x.Inscripcion)
                .WithMany(i => i.SeccionesCompletadas)
                .HasForeignKey(x => x.IdInscripcion);
            modelBuilder
                .Entity<SeccionCompletada>()
                .HasOne(x => x.Seccion)
                .WithMany()
                .HasForeignKey(x => x.IdSeccion);
        }
    }
}
