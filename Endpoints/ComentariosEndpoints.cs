using AutoMapper;
using Azure.Core.GeoJson;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Entidades;
using MinimalAPIPelicula.Migrations;
using MinimalAPIPelicula.Repositorios;

namespace MinimalAPIPelicula.Endpoints
{
    public static class ComentariosEndpoints
    {
        

        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapDelete("/{id:int}", Borrar);
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c  => 
                c.Expire(TimeSpan.FromSeconds(60))
                .Tag("Comentarios - get")
                .SetVaryByRouteValue(new string[] {"peliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear);
            group.MapPut("/{id:int}",Actualizar);


            return group;

        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId, IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas,IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();

            }

            var comentario = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentarioDTO = mapper.Map<List<ComentarioDTO>>(comentario);
            return TypedResults.Ok(comentarioDTO);


        }

        static async Task<Results<Ok<ComentarioDTO>,NotFound>> ObtenerPorId(int peliculaId, int id, IRepositorioComentarios repositorio, 
            IOutputCacheStore outputCacheStore,IMapper mapper)
        {
            var comentario = await repositorio.ObtenerPorId(id);

            if (comentario is null)
            { 
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);

        }

        static async Task<Results<Created<ComentarioDTO>, NotFound>> Crear( int peliculaId, CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPelicula, IMapper mapper,IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioPelicula.Existe(peliculaId))
            {
                return TypedResults.NotFound();

            }

            var comentario = mapper.Map<Entidades.Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("Comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/Comentarios/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent,NotFound>> Actualizar(int peliculaId, int id, IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas,
            IMapper mapper, IOutputCacheStore outputCacheStore, CrearComentarioDTO crearComentarioDTO)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Entidades.Comentario>(crearComentarioDTO);
            comentario.Id = id;
            comentario.PeliculaId = peliculaId;

            await repositorioComentarios.Actualizar(comentario);
            await outputCacheStore.EvictByTagAsync("Comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent,NotFound>> Borrar(int peliculaId, int id, IRepositorioComentarios repositorio,
            IOutputCacheStore outputCacheStore)
        {

            if (!await repositorio.Existe(id))
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("Comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
