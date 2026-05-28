using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Filters;
using Personal_Sitios.Models;
using Personal_Sitios.Repositories;
using Personal_Sitios.Services;
using Personal_Sitios.ViewModels;

namespace Personal_Sitios.Controllers
{
    [SessionAuthorize]
    [Route("Seguridad/Roles")]
    public class RolesController : Controller
    {
        private readonly RolesRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public RolesController(
            RolesRepository repository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [HttpGet("")]
        public IActionResult Index(int pagina = 1)
        {
            int cantidadPorPagina = 10;

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var modelo = new RolesListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Roles = _repository.ListarPaginado(pagina, cantidadPorPagina)
            };

            _bitacoraService.RegistrarConsulta(
                idUsuario,
                "Roles"
            );

            return View(modelo);
        }

        [HttpGet("Crear")]
        public IActionResult Crear()
        {
            var modelo = new RolViewModel
            {
                PantallasDisponibles = _repository.ListarPantallas()
            };

            return View(modelo);
        }

        [HttpPost("Crear")]
        public IActionResult Crear(RolViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.PantallasDisponibles = _repository.ListarPantallas();

                return View(modelo);
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var rol = new Rol
            {
                nombre = modelo.nombre,
                activo = modelo.activo
            };

            int idRol = _repository.Crear(
                rol,
                modelo.PantallasSeleccionadas
            );

            rol.id_rol = idRol;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Rol",
                new
                {
                    rol.id_rol,
                    rol.nombre,
                    rol.activo,
                    pantallas = modelo.PantallasSeleccionadas
                }
            );

            TempData["Exito"] = "Rol creado correctamente.";

            return RedirectToAction("Index");
        }

        [HttpGet("Editar/{id}")]
        public IActionResult Editar(int id)
        {
            var rol = _repository.ObtenerPorId(id);

            if (rol == null)
            {
                return RedirectToAction("Index");
            }

            var modelo = new RolViewModel
            {
                id_rol = rol.id_rol,
                nombre = rol.nombre,
                activo = rol.activo,
                PantallasDisponibles = _repository.ListarPantallas(),
                PantallasSeleccionadas = _repository.ObtenerPantallasDelRol(id)
            };

            return View(modelo);
        }

        [HttpPost("Editar/{id}")]
        public IActionResult Editar(int id, RolViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.PantallasDisponibles = _repository.ListarPantallas();

                return View(modelo);
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var rolAnterior = _repository.ObtenerPorId(id);
            var pantallasAnteriores = _repository.ObtenerPantallasDelRol(id);

            var rolActual = new Rol
            {
                id_rol = id,
                nombre = modelo.nombre,
                activo = modelo.activo
            };

            _repository.Actualizar(
                rolActual,
                modelo.PantallasSeleccionadas
            );

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Rol",
                new
                {
                    rolAnterior,
                    pantallas = pantallasAnteriores
                },
                new
                {
                    rolActual,
                    pantallas = modelo.PantallasSeleccionadas
                }
            );

            TempData["Exito"] = "Rol actualizado correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost("Eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var rol = _repository.ObtenerPorId(id);

            if (rol == null)
            {
                return RedirectToAction("Index");
            }

            if (!_repository.PuedeEliminar(id))
            {
                TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";

                return RedirectToAction("Index");
            }

            _repository.Eliminar(id);

            _bitacoraService.RegistrarDelete(
                idUsuario,
                "Rol",
                rol
            );

            TempData["Exito"] = "Rol eliminado correctamente.";

            return RedirectToAction("Index");
        }
    }
}