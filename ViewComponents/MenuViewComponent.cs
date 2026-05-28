using Microsoft.AspNetCore.Mvc;
using Personal_Sitios.Repositories;

namespace Personal_Sitios.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly MenuRepository _repository;

        public MenuViewComponent(MenuRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return View(new List<Models.Pantalla>());
            }

            var menu = _repository.ObtenerMenuPorUsuario(idUsuario.Value);

            return View(menu);
        }
    }
}