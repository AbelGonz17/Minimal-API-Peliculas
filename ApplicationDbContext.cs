using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MinimalAPIPelicula.Entidades;
using System.Security.Principal;

namespace MinimalAPIPelicula
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        //esto es api fluente
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().Property(p => p.Nombre).HasMaxLength(50);
            modelBuilder.Entity<Genero>().Property(p => p.Nombre).IsRequired();

            modelBuilder.Entity<Actor>().Property(p => p.Nombre).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(p => p.Foto).IsUnicode();

            modelBuilder.Entity<Pelicula>().Property(p => p.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Pelicula>().Property(p => p.Poster).IsUnicode();

            modelBuilder.Entity<GeneroPelicula>().HasKey(g => new { g.GeneroId,  g.PeliculaId});

            modelBuilder.Entity<ActorPelicula>().HasKey(a => new {a.Actorid, a.PeliculaId});
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<GeneroPelicula> GeneroPeliculas { get; set; }
        public DbSet<ActorPelicula> ActorPeliculas { get; set; }

    }
}
