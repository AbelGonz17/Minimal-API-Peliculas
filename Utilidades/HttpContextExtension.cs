using Microsoft.EntityFrameworkCore;

namespace MinimalAPIPelicula.Utilidades
{
    public static class HttpContextExtension
    {
        public async static Task InsetarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext is  null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Append("CantidadTotalRegistros", cantidad.ToString());
        }
    }
}
