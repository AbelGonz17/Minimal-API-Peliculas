using MinimalAPIPelicula.Entidades;

namespace MinimalAPIPelicula.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<List<Genero>> Obtenertodos();
        Task<Genero?> ObtenerPorId(int id);
        Task<int> crear(Genero genero);
        Task<bool> Existe(int id);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
        Task<List<int>> Existen(List<int> ids);
        Task<bool> Existe(int id, string nombre);
    }
}
