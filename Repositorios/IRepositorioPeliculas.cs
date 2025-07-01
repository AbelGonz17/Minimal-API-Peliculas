using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Entidades;

namespace MinimalAPIPelicula.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task AsiganarActores(int id, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}