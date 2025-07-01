using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MinimalAPIPelicula.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        //a esto null! se le llama perdonar el nulo 
        //esta tecnica se llama anotaciones de dato
        //[StringLength(50)]
        public string Nombre { get; set; } = null!;
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();
    }
}
