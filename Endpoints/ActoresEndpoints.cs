using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Entidades;
using MinimalAPIPelicula.Repositorios;
using MinimalAPIPelicula.Servicios;

namespace MinimalAPIPelicula.Endpoints
{
    public static class ActoresEndpoints
    {
        private static readonly string contenedor = "Actores";

        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapPost("/", CrearActor).DisableAntiforgery();
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery();
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("Actores-get"));
            group.MapGet("obtenerPorNombre/{Nombre}", ObtenerPorNombre);
            group.MapGet("/{id:int}", ObtenerActorPorId);
            group.MapDelete("/{id:int}", BorrarActor);

            return group;
            
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerTodos(IRepositorioActores repositorio, IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);

        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerActorPorId(IRepositorioActores repositorio, int id, IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorId(id);

            if (actores is null)
            {
                return TypedResults.NotFound();
            }

            var actoresDTO = mapper.Map<ActorDTO>(actores);

            return TypedResults.Ok(actoresDTO);

        }


        static async Task<Results<Created<ActorDTO>,ValidationProblem>> CrearActor([FromForm] CrearActorDTO crearActorDTO, 
            IRepositorioActores repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos,IValidator<CrearActorDTO> validator)
        { 

            var resultadoValidacion = await validator.ValidateAsync(crearActorDTO);

            if(!resultadoValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());

            }

            var actor = mapper.Map<Actor>(crearActorDTO);

            if(crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }

            var id = await repositorio.crear(actor);
            await outputCacheStore.EvictByTagAsync("Actores-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);

            return TypedResults.Created($"/Actores/{id}", actorDTO);

        }

        static async Task<Results<NoContent, NotFound>> BorrarActor(int id, IRepositorioActores repositorio, IOutputCacheStore outputCacheStore,IAlmacenadorArchivos almacenadorArchivos)
        {
            var actorDB = await repositorio.ObtenerPorId(id);

            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }
            var existe = await repositorio.Existe(id);

            

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(actorDB.Foto,contenedor);
            await outputCacheStore.EvictByTagAsync("Actores-get", default);
            return TypedResults.NoContent();


        }


        static async Task<Results<NoContent,NotFound>> Actualizar(int id, [FromForm] CrearActorDTO crearActorDTO,IRepositorioActores repositorio, IAlmacenadorArchivos almacenadorArchivos,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var actorDB = await repositorio.ObtenerPorId(id);
         

            if(actorDB is null)
            {
                return TypedResults.NotFound();
            }

            var actorParaActualizar = mapper.Map<Actor>(crearActorDTO);
            actorParaActualizar.id = id;
            actorParaActualizar.Foto = actorDB.Foto;

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Editar(actorParaActualizar.Foto, contenedor, crearActorDTO.Foto);
                actorParaActualizar.Foto = url;
            }

            await repositorio.Actualizar(actorParaActualizar);
            await outputCacheStore.EvictByTagAsync("Actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerPorNombre(string nombre, IRepositorioActores repositorio, IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorNombre(nombre);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);

        }
    }
}
