using System.Reflection.Metadata.Ecma335;

namespace MinimalAPIPelicula.Entidades
{
    public class ActorPelicula
    {
        public  int PeliculaId{ get; set; }
        public int Actorid { get; set; }
        public Actor Actor { get; set; } = null!;
        public Pelicula Pelicula { get; set; } = null!; 
        public int Orden {  get; set; }
        public string  Personaje { get; set; } = null!;
    }
}
