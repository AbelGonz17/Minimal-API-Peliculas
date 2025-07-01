using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Entidades;
using MinimalAPIPelicula.Migrations;
using MinimalAPIPelicula.Repositorios;
using MinimalAPIPelicula.Servicios;

namespace MinimalAPIPelicula.Endpoints
{
    public static class PeliculasEndPoints
    {
        private static readonly string contenedor = "Peliculas";

        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapPost("/", CrearPelicula).DisableAntiforgery();
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("Peliculas-get"));
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery();
            group.MapGet("/ {id:int}", ObtenerPeliculaPorId);
            group.MapDelete("/{id:int}", BorrarPelicula);
            group.MapPost("/ {id:int} /AsignarGeneros", AsignarGeneros);
            group.MapPost("/ {id:int} /AsignarActores", AsignarActores);



            return group;

        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var PeliculaDB = await repositorio.ObtenerPorId(id);


            if (PeliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = PeliculaDB.Poster;

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster, contenedor, crearPeliculaDTO.Poster);
                peliculaParaActualizar.Poster = url;
            }

            await repositorio.Actualizar(peliculaParaActualizar);
            await outputCacheStore.EvictByTagAsync("Peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarPelicula(int id, IRepositorioPeliculas repositorio, IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }
            var existe = await repositorio.Existe(id);



            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("Peliculas-get", default);
            return TypedResults.NoContent();


        }

        static async Task<Ok<List<PeliculaDto>>> ObtenerTodos(IRepositorioPeliculas repositorio, IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var pelicula = await repositorio.ObtenerTodos(paginacion);
            var peliculaDTO = mapper.Map<List<PeliculaDto>>(pelicula);
            return TypedResults.Ok(peliculaDTO);

        }

        static async Task<Results<Ok<PeliculaDto>, NotFound>> ObtenerPeliculaPorId(IRepositorioPeliculas repositorio, int id, IMapper mapper)
        {
            var peliculas = await repositorio.ObtenerPorId(id);

            if (peliculas is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDto>(peliculas);

            return TypedResults.Ok(peliculaDTO);

        }

        static async Task<Created<PeliculaDto>> CrearPelicula([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorio.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("Actores-get", default);
            var peliculaDTO = mapper.Map<PeliculaDto>(pelicula);

            return TypedResults.Created($"/Peliculas/{id}", peliculaDTO);

        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generoIds, IRepositorioGeneros repositorioGeneros, IRepositorioPeliculas repositorioPeliculas)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();

            if(generoIds.Count != 0)
            {
                generosExistentes = await repositorioGeneros.Existen(generoIds);
            }

            if( generosExistentes.Count != generoIds.Count)
            {
                var generosNoExistentes = generoIds.Except(generosExistentes);

                return TypedResults.BadRequest($"Los generos de id {string.Join(",", generosNoExistentes)} no existen.");
            }

            await repositorioPeliculas.AsignarGeneros(id,generoIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound,NoContent,BadRequest<string>>>AsignarActores(int id, List<AsignarActorPeliculaDTO> actoresDTO,
            IRepositorioActores repositorioActores, IRepositorioPeliculas repositorioPeliculas , IMapper mapper )
        {
            if( !await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound(); 
            }

            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.Actorid).ToList();

            if(actoresDTO.Count != 0)
            {
                actoresExistentes = await repositorioActores.Existen(actoresIds);
            }

            if(actoresExistentes.Count != actoresDTO.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);

                return TypedResults.BadRequest($"Los actores de id {string.Join(",", actoresNoExistentes)} no existen.");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositorioPeliculas.AsiganarActores(id, actores);
            return TypedResults.NoContent();
        }
    }
}
