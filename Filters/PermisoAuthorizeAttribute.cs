using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Personal_Sitios.Repositories;

namespace Personal_Sitios.Filters
{
    public class PermisoAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly PermisosRepository _repository;

        public PermisoAuthorizeAttribute(PermisosRepository repository)
        {
            _repository = repository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int? idUsuario =
                context.HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                context.Result =
                    new RedirectToActionResult("Index", "Login", null);

                return;
            }

            string rutaActual =
                context.HttpContext.Request.Path.Value ?? "";

            var rutasPermitidas =
                _repository.ObtenerRutasPermitidas(idUsuario.Value);

            bool tienePermiso =
                rutasPermitidas.Any(ruta =>
                    !string.IsNullOrWhiteSpace(ruta)
                    &&
                    rutaActual.StartsWith(
                        ruta,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            if (!tienePermiso)
            {
                var controller =
                    context.Controller as Controller;

                if (controller != null)
                {
                    controller.TempData["Error"] =
                        "No tiene permisos para acceder a esta pantalla.";
                }

                context.Result =
                    new RedirectToActionResult("Index", "Home", null);

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}