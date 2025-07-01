using System.Data;

namespace MinimalAPIPelicula.Entidades
{
    public class Actor
    {
        public int id{ get; set; }
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
        public List<ActorPelicula> ActoresPeliculas { get; set; } = new List<ActorPelicula>();
    }
}
