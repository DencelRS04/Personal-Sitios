using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Repositories;
using Personal_Sitios.ViewModels;

namespace Personal_Sitios.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginRepository _repository;

        public LoginController(LoginRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario =
                _repository.ObtenerUsuario(model.Usuario);

            if (usuario == null)
            {
                ViewBag.Error =
                    "Usuario y/o contraseña incorrectos";

                return View(model);
            }

            bool passwordCorrecto =
                model.Password == usuario.password_hash;

            if (!passwordCorrecto)
            {
                ViewBag.Error =
                    "Usuario y/o contraseña incorrectos";

                return View(model);
            }

            HttpContext.Session.SetString(
                "Usuario",
                usuario.nombre_completo
            );

            HttpContext.Session.SetInt32(
                "IdUsuario",
                usuario.id_usuario
            );

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Login"
            );
        }
    }
}