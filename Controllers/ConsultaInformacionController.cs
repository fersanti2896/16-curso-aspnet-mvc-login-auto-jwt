using Dominio.IServicios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Transporte;

namespace Pedidos.Controllers {
    public class ConsultaInformacionController : Controller {
        private readonly IConsultaPedidoService _IConsultaPedidoService;

        public ConsultaInformacionController(IConsultaPedidoService iConsultaPedidoService) {
            _IConsultaPedidoService = iConsultaPedidoService;
        }

        [HttpPost]
        public async Task<ActionResult> ConsultaPedidosGeneral(ModeloConsultaDTO modelo) {
            /* Se deberá tomar de JWT */
            modelo.CatPerfilId = 2;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var lista = await _IConsultaPedidoService.ConsultaPedidosGeneral(modelo);

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            return PartialView(lista);
        }

        [HttpPost]
        public async Task<JsonResult> ConsultaEstadosPedidos() {
            var lista = await _IConsultaPedidoService.ConsultaEstadosPedidos();

            return Json(lista); 
        }

        [HttpPost]
        public IActionResult EliminarPedido(PedidoDTO modelo) {
            var resultado = _IConsultaPedidoService.EliminarPedido(modelo.PedidoId);

            return resultado ? Ok() : StatusCode(500, "Ocurrió un error");
        }

        [HttpPost]
        public IActionResult ActualizarEstadoPedido(PedidoDTO modelo) {
            var resultado = _IConsultaPedidoService.ActualizarEstadoPedido(modelo);

            return resultado ? Ok() : StatusCode(500, "Ocurrió un error");
        }

        [HttpPost]
        public IActionResult ConsultaDetallePedido(PedidoEdoDTO modelo) {
            _IConsultaPedidoService.ConsultaDetallePedido(modelo);

            return PartialView(modelo);
        }

        [HttpPost]
        public JsonResult ConsultaProductos() {
            var lista = _IConsultaPedidoService.ConsultaProductos().ToList();

            return Json(lista);
        }

        [HttpPost]
        public JsonResult ConsultaEstadosRepublica() {
            var lista = _IConsultaPedidoService.ConsultaEstadosRepublica().ToList();

            return Json(lista);
        }

        [HttpPost]
        public async Task<IActionResult> ConsultaDetallesDelPedido(PedidoEdoDTO modelo) {
            var lista = await _IConsultaPedidoService.ConsultaDetallesDelPedido(modelo);

            return PartialView(lista);
        }

        [HttpPost]
        public JsonResult GuardadoPedido(PedidoEdoDTO modelo) {
            var pedidoId = _IConsultaPedidoService.GuardadoPedido(modelo);

            return Json(pedidoId);
        }

        [HttpPost]
        public JsonResult EliminarPedidoDetalle(PedidoDTO modelo) {
            var resultado = _IConsultaPedidoService.EliminarPedidoDetalle(modelo);

            return Json(resultado);
        }
        
    }
}
