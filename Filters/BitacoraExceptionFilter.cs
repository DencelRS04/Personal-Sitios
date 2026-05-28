using Microsoft.AspNetCore.Mvc.Filters;
using Personal_Sitios.Services;

namespace Personal_Sitios.Filters
{
    public class BitacoraExceptionFilter : IExceptionFilter
    {
        private readonly BitacoraService _bitacoraService;

        public BitacoraExceptionFilter(BitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        public void OnException(ExceptionContext context)
        {
            int? idUsuario =
                context.HttpContext.Session.GetInt32("IdUsuario");

            string controller =
                context.RouteData.Values["controller"]?.ToString()
                ?? "Desconocido";

            string action =
                context.RouteData.Values["action"]?.ToString()
                ?? "Desconocido";

            string entidad =
                $"{controller}/{action}";

            string error =
                context.Exception.Message;

            _bitacoraService.RegistrarError(
                idUsuario,
                entidad,
                error
            );
        }
    }
}