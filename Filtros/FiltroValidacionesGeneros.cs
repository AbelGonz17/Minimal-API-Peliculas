
using FluentValidation;
using MinimalAPIPelicula.DTOs;

namespace MinimalAPIPelicula.Filtros
{
    public class FiltroValidacionesGeneros : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validador = context.HttpContext.RequestServices.GetService<IValidator<CrearGeneroDTO>>();

            if(validador is null)
            {
                return await next (context);
            }

            var insumoAvalidar = context.Arguments.OfType<CrearGeneroDTO>().FirstOrDefault();   

            if(insumoAvalidar is null)
            {
                return TypedResults.Problem("no pudo ser encontrada la entidad a validar");
            }

            var resultadoValidacion = await validador.ValidateAsync(insumoAvalidar);

            if (!resultadoValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());
            }

            return await next(context);
        }
    }
}
