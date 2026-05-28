using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Filters;
using Personal_Sitios.Models;
using Personal_Sitios.Repositories;
using Personal_Sitios.Services;
using Personal_Sitios.ViewModels;

namespace Personal_Sitios.Controllers
{
    [SessionAuthorize]
    [Route("Seguridad/Modulos")]
    public class PantallasController : Controller
    {
        private readonly PantallasRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public PantallasController(
            PantallasRepository repository,
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

            var modelo = new PantallasListaViewModel
            {
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(),
                Pantallas = _repository.ListarPaginado(
                    pagina,
                    cantidadPorPagina
                )
            };

            _bitacoraService.RegistrarConsulta(
                idUsuario,
                "Pantallas"
            );

            return View(modelo);
        }

        [HttpGet("Crear")]
        public IActionResult Crear()
        {
            var modelo = new PantallaViewModel
            {
                RolesDisponibles = _repository.ListarRoles()
            };

            return View(modelo);
        }

        [HttpPost("Crear")]
        public IActionResult Crear(PantallaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles = _repository.ListarRoles();

                return View(modelo);
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var pantalla = new Pantalla
            {
                nombre = modelo.nombre,

                // Datos técnicos internos para que la pantalla pueda existir en el menú.
                // No se muestran en la vista porque la HU solo solicita nombre y roles.
                modulo = "Seguridad",
                ruta = "#",
                icono = "fa-window-maximize",
                orden_menu = 99,
                visible_menu = true,
                activo = true
            };

            int idPantalla = _repository.Crear(
                pantalla,
                modelo.RolesSeleccionados
            );

            pantalla.id_pantalla = idPantalla;

            _bitacoraService.RegistrarInsert(
                idUsuario,
                "Pantalla",
                new
                {
                    pantalla.id_pantalla,
                    pantalla.nombre,
                    roles = modelo.RolesSeleccionados
                }
            );

            TempData["Exito"] = "Pantalla creada correctamente.";

            return RedirectToAction("Index");
        }

        [HttpGet("Editar/{id}")]
        public IActionResult Editar(int id)
        {
            var pantalla = _repository.ObtenerPorId(id);

            if (pantalla == null)
            {
                return RedirectToAction("Index");
            }

            var modelo = new PantallaViewModel
            {
                id_pantalla = pantalla.id_pantalla,
                nombre = pantalla.nombre,
                RolesDisponibles = _repository.ListarRoles(),
                RolesSeleccionados = _repository.ObtenerRolesDePantalla(id)
            };

            return View(modelo);
        }

        [HttpPost("Editar/{id}")]
        public IActionResult Editar(int id, PantallaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles = _repository.ListarRoles();

                return View(modelo);
            }

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var pantallaAnterior = _repository.ObtenerPorId(id);
            var rolesAnteriores = _repository.ObtenerRolesDePantalla(id);

            var pantallaActual = new Pantalla
            {
                id_pantalla = id,
                nombre = modelo.nombre
            };

            _repository.Actualizar(
                pantallaActual,
                modelo.RolesSeleccionados
            );

            _bitacoraService.RegistrarUpdate(
                idUsuario,
                "Pantalla",
                new
                {
                    pantallaAnterior,
                    roles = rolesAnteriores
                },
                new
                {
                    pantallaActual,
                    roles = modelo.RolesSeleccionados
                }
            );

            TempData["Exito"] = "Pantalla actualizada correctamente.";

            return RedirectToAction("Index");
        }

        [HttpPost("Eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            var pantalla = _repository.ObtenerPorId(id);

            if (pantalla == null)
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
                "Pantalla",
                pantalla
            );

            TempData["Exito"] = "Pantalla eliminada correctamente.";

            return RedirectToAction("Index");
        }
    }
}