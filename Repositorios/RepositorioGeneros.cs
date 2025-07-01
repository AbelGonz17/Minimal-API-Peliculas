using Microsoft.EntityFrameworkCore;
using MinimalAPIPelicula.Entidades;

namespace MinimalAPIPelicula.Repositorios
{
    public class RepositorioGenerosL : IRepositorioGeneros
    {
        private readonly ApplicationDbContext context;

        public RepositorioGenerosL(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Genero>> Obtenertodos()
        {
            return await context.Generos.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<int> crear(Genero genero)
        {
            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }

        public async Task Borrar(int id)
        {
            await context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task Actualizar(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();

        }

        public async Task<bool> Existe(int id, string nombre)
        {
            return await context.Generos.AnyAsync(g => g.Id != id && g.Nombre ==  nombre);
        }
       
        public async Task<bool> Existe(int id)
        {
            return await context.Generos.AnyAsync(x=> x.Id == id);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Generos.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();  

        } 
    }
}
