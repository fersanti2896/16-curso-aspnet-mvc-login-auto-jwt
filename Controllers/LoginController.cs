using Dominio.IServicios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transporte;

namespace Pedidos.Controllers {
    public class LoginController : Controller {
        private readonly IUsuarioService _usuarioService;

        public LoginController(IUsuarioService usuarioService) { 
            _usuarioService = usuarioService;
        }
        public IActionResult Index() {
            ViewBag.SinSesion = "OK";
            return View();
        }

        /* Valida las credenciales */
        [HttpPost]
        public async Task<JsonResult> ValidacionCredenciales(UsuarioDTO modelo) {
            try {
                var usuario = await _usuarioService.ValidacionCredenciales(modelo);

                if (usuario == null) {
                    return Json(false);
                }

                var token = "token";

                return Json(token);
            }
            catch (System.Exception) {
                return Json(false);
            }
        }
    }
}
