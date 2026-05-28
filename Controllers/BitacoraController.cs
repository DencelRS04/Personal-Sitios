using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Filters;
using Personal_Sitios.Repositories;
using Personal_Sitios.Services;
using Personal_Sitios.ViewModels;

namespace Personal_Sitios.Controllers
{
    [SessionAuthorize]
    [Route("General/Bitacora")]
    public class BitacoraController : Controller
    {
        private readonly BitacoraRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public BitacoraController(
            BitacoraRepository repository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        [HttpGet("")]
        public IActionResult Index(
    string usuarioFiltro,
    string descripcionFiltro,
    string orden = "fecha_desc",
    int pagina = 1)
        {
            int cantidadPorPagina = 100;

            var modelo = new BitacoraFiltroViewModel
            {
                UsuarioFiltro = usuarioFiltro,
                DescripcionFiltro = descripcionFiltro,
                Orden = orden,
                Pagina = pagina,
                CantidadPorPagina = cantidadPorPagina,
                TotalRegistros = _repository.Contar(usuarioFiltro, descripcionFiltro),
                Bitacoras = _repository.Listar(
                    usuarioFiltro,
                    descripcionFiltro,
                    orden,
                    pagina,
                    cantidadPorPagina
                )
            };

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            _bitacoraService.RegistrarConsulta(
                idUsuario,
                "Bitácora"
            );

            return View(modelo);
        }
    }
}