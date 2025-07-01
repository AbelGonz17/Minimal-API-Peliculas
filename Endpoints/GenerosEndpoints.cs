using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Validations;
using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Entidades;
using MinimalAPIPelicula.Filtros;
using MinimalAPIPelicula.Migrations;
using MinimalAPIPelicula.Repositorios;

namespace MinimalAPIPelicula.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", obtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

            group.MapGet("/{id:int}", ObtenerGeneroPorId);

            group.MapPost("/", CrearGenero).AddEndpointFilter<FiltroValidacionesGeneros>();

            group.MapPut("/{id:int}", ActualizaGenero).AddEndpointFilter<FiltroValidacionesGeneros>();

            group.MapDelete("/{id:int}", BorrarGenero);
            return group;
        }

        static async Task<Ok<List<GeneroDTO>>> obtenerGeneros(IRepositorioGeneros repositorio,IMapper mapper)
        {
            var generos = await repositorio.Obtenertodos();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDTO);

        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = mapper.Map<GeneroDTO>(genero);

            return TypedResults.Ok(generoDTO);

        }

        static async Task<Results<Created<GeneroDTO>,ValidationProblem>> CrearGenero(CrearGeneroDTO crearGeneroDTO , IRepositorioGeneros repositorio, 
            IOutputCacheStore outputCacheStore,IMapper mapper)
        {
            

            var genero = mapper.Map<Genero>(crearGeneroDTO);         
            var id = await repositorio.crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO= mapper.Map<GeneroDTO>(genero);


            return TypedResults.Created($"/generos/{id}", generoDTO);

        }

        static async Task<Results<NoContent, NotFound,ValidationProblem>> ActualizaGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorio
            , IOutputCacheStore outputCacheStore,IMapper mapper)
        {
     

            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;


            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();


        }

    }
}
