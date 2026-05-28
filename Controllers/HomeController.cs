using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Filters;

namespace Personal_Sitios.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.NombreCompleto =
                HttpContext.Session.GetString("NombreCompleto");

            ViewBag.Usuario =
                HttpContext.Session.GetString("Usuario");

            return View();
        }
    }
}