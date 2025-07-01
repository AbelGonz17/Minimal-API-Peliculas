namespace MinimalAPIPelicula.DTOs
{
    public class GeneroDTO
    {
        public int Id { get; set; }
        //a esto null! se le llama perdonar el nulo 
        //esta tecnica se llama anotaciones de dato
        //[StringLength(50)]
        public string Nombre { get; set; } = null!;

    }
}
