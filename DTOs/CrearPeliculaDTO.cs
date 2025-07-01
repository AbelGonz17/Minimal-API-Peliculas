namespace MinimalAPIPelicula.DTOs
{
    public class CrearPeliculaDTO
    {

        
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
