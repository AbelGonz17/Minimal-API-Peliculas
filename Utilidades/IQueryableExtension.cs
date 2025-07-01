using MinimalAPIPelicula.DTOs;
using System.Runtime.CompilerServices;

namespace MinimalAPIPelicula.Utilidades
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable,PaginacionDTO paginacionDTO )
        {
            return queryable.Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina).
                Take(paginacionDTO.RecordsPorPagina);
            
        }
    }
}
