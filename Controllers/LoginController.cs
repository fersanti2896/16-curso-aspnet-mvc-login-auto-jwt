using Dominio.IServicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pedidos.Utils;
using System.Threading.Tasks;
using Transporte;

namespace Pedidos.Controllers {
    public class LoginController : Controller {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _config;

        public LoginController(IUsuarioService usuarioService, IConfiguration configuration) { 
            _usuarioService = usuarioService;
            _config = configuration;    
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

                var token = JwtConfigurator.GetToken(usuario, _config);

                return Json(token);
            }
            catch (System.Exception) {
                return Json(false);
            }
        }
    }
}
