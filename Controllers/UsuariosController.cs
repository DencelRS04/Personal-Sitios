using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Filters;
using Personal_Sitios.Helpers;
using Personal_Sitios.Models;
using Personal_Sitios.Repositories;
using Personal_Sitios.Services;
using Personal_Sitios.ViewModels;

namespace Personal_Sitios.Controllers
{
    [SessionAuthorize]
    [ServiceFilter(typeof(PermisoAuthorizeAttribute))]
    [Route("Seguridad/Usuarios")]
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository _repository;
        private readonly BitacoraService _bitacoraService;
        private readonly EncryptionHelper _encryptionHelper;

        public UsuariosController(
            UsuariosRepository repository,
            BitacoraService bitacoraService,
            EncryptionHelper encryptionHelper)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
            _encryptionHelper = encryptionHelper;
        }

        [HttpGet("")]
        public IActionResult Index(int pagina = 1)
        {
            int cantidadPorPagina = 10;

            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            var modelo = new UsuariosListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Usuarios = _repository.ListarPaginado(
                    pagina,
                    cantidadPorPagina
                )
            };

            _bitacoraService.RegistrarConsulta(
                idUsuario,
                "Usuarios"
            );

            return View(modelo);
        }

        [HttpGet("Crear")]
        public IActionResult Crear()
        {
            var modelo = new UsuarioViewModel
            {
                estado = "ACTIVO",
                RolesDisponibles = _repository.ListarRoles()
            };

            return View(modelo);
        }

        [HttpPost("Crear")]
        public IActionResult Crear(UsuarioViewModel modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.password))
            {
                ModelState.AddModelError(
                    "password",
                    "La contraseña es obligatoria"
                );
            }

            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles =
                    _repository.ListarRoles();

                return View(modelo);
            }

            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            var usuario = new Usuario
            {
                usuario = modelo.usuario,
                nombre_completo = modelo.nombre_completo,
                correo = modelo.correo,
                password_hash = _encryptionHelper.Encriptar(
                    modelo.password
                ),
                estado = modelo.estado
            };

            int idNuevoUsuario =
                _repository.Crear(
                    usuario,
                    modelo.RolesSeleccionados
                );

            usuario.id_usuario = idNuevoUsuario;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Usuario",
                new
                {
                    usuario.id_usuario,
                    usuario.usuario,
                    usuario.nombre_completo,
                    usuario.correo,
                    usuario.estado,
                    roles = modelo.RolesSeleccionados
                }
            );

            TempData["Exito"] =
                "Usuario creado correctamente.";

            return RedirectToAction("Index");
        }

        [HttpGet("Editar/{id}")]
        public IActionResult Editar(int id)
        {
            var usuario =
                _repository.ObtenerPorId(id);

            if (usuario == null)
            {
                return RedirectToAction("Index");
            }

            var modelo = new UsuarioViewModel
            {
                id_usuario = usuario.id_usuario,
                usuario = usuario.usuario,
                nombre_completo = usuario.nombre_completo,
                correo = usuario.correo,
                estado = usuario.estado,
                RolesDisponibles = _repository.ListarRoles(),
                RolesSeleccionados =
                    _repository.ObtenerRolesDelUsuario(id)
            };

            return View(modelo);
        }

        [HttpPost("Editar/{id}")]
        public IActionResult Editar(int id, UsuarioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles =
                    _repository.ListarRoles();

                return View(modelo);
            }

            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            var usuarioAnterior =
                _repository.ObtenerPorId(id);

            if (usuarioAnterior == null)
            {
                return RedirectToAction("Index");
            }

            var rolesAnteriores =
                _repository.ObtenerRolesDelUsuario(id);

            string passwordFinal =
                usuarioAnterior.password_hash;

            if (!string.IsNullOrWhiteSpace(modelo.password))
            {
                passwordFinal =
                    _encryptionHelper.Encriptar(
                        modelo.password
                    );
            }

            var usuarioActual = new Usuario
            {
                id_usuario = id,
                usuario = modelo.usuario,
                nombre_completo = modelo.nombre_completo,
                correo = modelo.correo,
                password_hash = passwordFinal,
                estado = modelo.estado
            };

            _repository.Actualizar(
                usuarioActual,
                modelo.RolesSeleccionados
            );

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Usuario",
                new
                {
                    usuarioAnterior.id_usuario,
                    usuarioAnterior.usuario,
                    usuarioAnterior.nombre_completo,
                    usuarioAnterior.correo,
                    usuarioAnterior.estado,
                    roles = rolesAnteriores
                },
                new
                {
                    usuarioActual.id_usuario,
                    usuarioActual.usuario,
                    usuarioActual.nombre_completo,
                    usuarioActual.correo,
                    usuarioActual.estado,
                    roles = modelo.RolesSeleccionados
                }
            );

            TempData["Exito"] =
                "Usuario actualizado correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost("Eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            var usuario =
                _repository.ObtenerPorId(id);

            if (usuario == null)
            {
                return RedirectToAction("Index");
            }

            if (!_repository.PuedeEliminar(id))
            {
                TempData["Error"] =
                    "No se puede eliminar un registro con datos relacionados.";

                return RedirectToAction("Index");
            }

            _repository.Eliminar(id);

            _bitacoraService.RegistrarDelete(
                idUsuario,
                "Usuario",
                new
                {
                    usuario.id_usuario,
                    usuario.usuario,
                    usuario.nombre_completo,
                    usuario.correo,
                    usuario.estado
                }
            );

            TempData["Exito"] =
                "Usuario eliminado correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost("CambiarEstado/{id}")]
        public IActionResult CambiarEstado(int id)
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            var usuario =
                _repository.ObtenerPorId(id);

            if (usuario == null)
            {
                return RedirectToAction("Index");
            }

            if (usuario.estado == "BLOQUEADO")
            {
                TempData["Error"] =
                    "No se puede activar o inactivar un usuario bloqueado.";

                return RedirectToAction("Index");
            }

            string nuevoEstado =
                usuario.estado == "ACTIVO"
                    ? "INACTIVO"
                    : "ACTIVO";

            _repository.CambiarEstado(
                id,
                nuevoEstado
            );

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Usuario",
                new
                {
                    usuario.id_usuario,
                    usuario.usuario,
                    usuario.nombre_completo,
                    usuario.correo,
                    usuario.estado
                },
                new
                {
                    usuario.id_usuario,
                    usuario.usuario,
                    usuario.nombre_completo,
                    usuario.correo,
                    estado = nuevoEstado
                }
            );

            TempData["Exito"] =
                "Estado del usuario actualizado correctamente.";

            return RedirectToAction("Index");
        }
    }
}