namespace MinimalAPIPelicula.Validaciones
{
    public static class Utilidades
    {
        public static  string CampoRequeridoMensaje = "El campo {PropertyName} es requerido";
        public static string MaximumLengthMensaje = "El campo {PropertyName} debe tener menos de { MaxLength } caracteres";
        public static string ComenzarConMayusMensaje = "El campo {PropertyName} debe comenzar con mayusculas";

        public static bool PrimeraLetraEnMayusculas(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();

            return primeraLetra == primeraLetra.ToUpper();
        }

        public static string GreatherThanOrEqualToMensaje(DateTime fechaMinima)
        {
           return ("El campo {PropertyName} debe ser posterior a " + fechaMinima.ToString("yyyy-MM-dd"));
        }
    }
}
