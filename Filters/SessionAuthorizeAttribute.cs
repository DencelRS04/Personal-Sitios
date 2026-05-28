using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Personal_Sitios.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var idUsuario = context.HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                var controller = context.Controller as Controller;

                if (controller != null)
                {
                    controller.TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
                }

                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}