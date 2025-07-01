using FluentValidation;
using MinimalAPIPelicula.DTOs;
using MinimalAPIPelicula.Repositorios;

namespace MinimalAPIPelicula.Validaciones
{
    public class CrearGeneroDTOValidador:AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador(IRepositorioGeneros repositorioGeneros,
            IHttpContextAccessor httpContextAccessor)
        {
            var valorDeRutaId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if(valorDeRutaId is string valorString)
            {
                int.TryParse(valorString, out id );
            }


            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(50).WithMessage(Utilidades.MaximumLengthMensaje)
                .Must(Utilidades.PrimeraLetraEnMayusculas).WithMessage(Utilidades.ComenzarConMayusMensaje)
                .MustAsync(async (nombre, _) =>
                {
                    var existe = await repositorioGeneros.Existe(id, nombre);

                    return !existe;
                }).WithMessage(g => $"Ya existe un genero con el nombre {g.Nombre}");         
        }

      
    }
}
