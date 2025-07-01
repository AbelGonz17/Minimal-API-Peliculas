using Microsoft.EntityFrameworkCore;
using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Entidades;
using MinimalAPIPelicula.Utilidades;

namespace MinimalAPIPelicula.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioActores(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await httpContext.InsetarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await context.Actores.AsNoTracking().FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            return await context.Actores
                .Where(a => a.Nombre.Contains(nombre))
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<int> crear(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Actores.AnyAsync(x => x.id == id);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Actores.Where(a => ids.Contains(a.id)).Select(a => a.id ).ToListAsync();
        }

        public async Task Actualizar(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();

        }

        public async Task Borrar(int id)
        {
            await context.Actores.Where(x => x.id == id).ExecuteDeleteAsync();
        }
    }
}

