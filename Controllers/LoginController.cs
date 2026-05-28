using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Helpers;
using Personal_Sitios.Repositories;
using Personal_Sitios.ViewModels;

namespace Personal_Sitios.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginRepository _repository;
        private readonly EncryptionHelper _encryptionHelper;

        public LoginController(
            LoginRepository repository,
            EncryptionHelper encryptionHelper)
        {
            _repository = repository;
            _encryptionHelper = encryptionHelper;
        }

        public IActionResult Index()
        {
            if (TempData["Mensaje"] != null)
            {
                ViewBag.Mensaje = TempData["Mensaje"];
            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error =
                    "Usuario y/o contraseña incorrectos.";

                return View(model);
            }

            var usuario =
                _repository.ObtenerUsuario(model.Usuario);

            if (usuario == null)
            {
                ViewBag.Error =
                    "Usuario y/o contraseña incorrectos.";

                return View(model);
            }

            if (usuario.estado == "BLOQUEADO")
            {
                ViewBag.Error =
                    "El usuario se encuentra bloqueado.";

                return View(model);
            }

            if (usuario.estado == "INACTIVO")
            {
                ViewBag.Error =
                    "El usuario se encuentra inactivo.";

                return View(model);
            }

            bool passwordCorrecto =
                _encryptionHelper.Verificar(
                    model.Password,
                    usuario.password_hash
                );

            if (!passwordCorrecto)
            {
                _repository.AumentarIntentos(
                    usuario.id_usuario
                );

                int intentosActuales =
                    usuario.intentos_login + 1;

                if (intentosActuales >= 3)
                {
                    _repository.BloquearUsuario(
                        usuario.id_usuario
                    );

                    ViewBag.Error =
                        "El usuario se bloqueó por fallar 3 intentos.";
                }
                else
                {
                    ViewBag.Error =
                        "Usuario y/o contraseña incorrectos.";
                }

                return View(model);
            }

            _repository.ReiniciarIntentos(
                usuario.id_usuario
            );

            HttpContext.Session.SetInt32(
                "IdUsuario",
                usuario.id_usuario
            );

            HttpContext.Session.SetString(
                "Usuario",
                usuario.usuario
            );

            HttpContext.Session.SetString(
                "NombreCompleto",
                usuario.nombre_completo
            );

            Response.Cookies.Append(
                "SesionIniciada",
                "1",
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Expires = DateTimeOffset.Now.AddHours(8)
                }
            );

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            Response.Cookies.Delete(
                "SesionIniciada"
            );

            TempData["Mensaje"] =
                "Sesión cerrada correctamente.";

            return RedirectToAction(
                "Index",
                "Login"
            );
        }
    }
}