const { parseHTML, error } = require("jquery");

function AgregarPedido(PedidoId) {
	ConsultaDetallePedido(PedidoId);
}

/* Para manejo de vista parcial */
function ConsultaDetallePedido(PedidoId) {
	$("#PedidoId").val(PedidoId);

	var modelo = { PedidoId: PedidoId };
	envioGenericos("/ConsultaInformacion/ConsultaDetallePedido", modelo, EdicionPedido);
}

/* Muestra un modal en el HTML parcial de ConsultaDetallePedido */
function EdicionPedido(html) {
	var pedido = parseInt($("#PedidoId").val());
	var title  = pedido == 0 ? "Nuevo Pedido" : "<strong>Edición del Pedido <u>" + pedido + "</u></strong>";
	var cerrar = false;

	Swal.fire({
		title: title,
		html: html,
		heightAuto: false,
		width: 1200,
		showCloseButton: true,
		showCancelButton: true,
		focusConfirm: false,
		closeModal: false,
		didOpen() {
			/* Se renderiza los productos y estados y se necesita que esté abierta */
			var modelo = {};

			envioGenericos("/ConsultaInformacion/ConsultaProductos", modelo, RenderizaProductos);
			envioGenericos("/ConsultaInformacion/ConsultaEstadosRepublica", modelo, RenderizaEstadosRepublica);

			$("#SelectProducto").on('change', function () {
				var imagen = $("#SelectProducto option:selected").text();
				$("#ImagenProducto").attr("src", "/images/" + imagen + ".jpg");
			});

			ConsultaDetallesDelPedido();

			Swal.keepOpened = true;
		},
		preConfirm: function (resultado) {
			/* Comportamiento detonadas por el botón confirmar */
			var errores = ValidaModelo();

			/* Valida error y muestra alerta */
			if (errores.length != 0) {
				MensajeError(errores);
			} else {
				EnvioGuardado();
			}

			return cerrar;
		},
		confirmButtonText: '<i class="far fa-save"></i> Guardar',
		cancelButtonText: '<i class="far fa-eraser"></i> Cerrar'
	})
}

function ActualizarEstadoPedido(PedidoId, EstadoSiguienteId) {
	var modelo = { PedidoId: PedidoId, CatEstadoPedidoId: EstadoSiguienteId }
	envioGenericos("/ConsultaInformacion/ActualizarEstadoPedido", modelo, ConsultaGrid);
}

function EliminarPedido(PedidoId) {
	var modelo = { PedidoId: PedidoId }
	envioGenericos("/ConsultaInformacion/EliminarPedido", modelo, ConsultaGrid);
}

function RenderizaProductos(jsonResult) {
	var opciones = RenderizaResultado(jsonResult);

	$("#SelectProducto").html(opciones);
}

function RenderizaEstadosRepublica(jsonResult) {
	var opciones = RenderizaResultado(jsonResult);

	$("#SelectEstadosRepublica").html(opciones);
}

function RenderizaResultado(dataJSON) {
	var select = "<option value='0'>SIN SELECCION</option>";

	for (var indice in dataJSON) {
		var llave = dataJSON[indice].key;
		var valor = dataJSON[indice].value;

		select += "<option value='"+ llave + "'>" + valor + "</option>"
	}

	return select;
}

function ConsultaDetallesDelPedido() {
	var modelo = { PedidoId: $("#PedidoId").val() }

	envioGenericos("/ConsultaInformacion/ConsultaDetallesDelPedido", modelo, "TablaDetallePedido")
}

function ValidaModelo() {
	var errores  = "";
	var PedidoId = parseInt($("#PedidoId").val());

	var CatEstadoRepublicaId = $("#SelectEstadosRepublica option:selected").val();
	var CatProductoId		 = $("#SelectProducto option:selected").val();
	var Cantidad = parseInt($("#PedidoCantidad").val());

	if (Cantidad < 0) Cantidad = 0;

	/* Validaciones: Se agrega un nuevo pedido */
	if (PedidoId == 0) {
		if (CatEstadoRepublicaId == 0) {
			errores += "-- Favor de Seleccionar el Estado de la República -- <br>";
		}

		if (Cantidad == 0) {
			errores += "-- Favor de Indicar la Cantidad -- <br>";
		}

		if (CatProductoId == 0) {
			errores += "-- Favor de Seleccionar el Producto -- <br>";
		}
	} else {
		/* En otro caso se agrega el pedido */
		if (CatProductoId == 0 && CatEstadoRepublicaId == 0) {
			errores += "-- Favor de Seleccionar el Estado de la República -- <br>";
			errores += "-- Favor de Seleccionar el Producto -- <br>";
		}

		if (CatProductoId != 0 && Cantidad == 0) {
			errores += "-- Favor de Indicar la Cantidad -- <br>";
        }
	}

	return errores;
}

/* Recibe un resultado */
function MuestraResultado(PedidoId) {
	$("#PedidoId").val(PedidoId);
	$("#swal2-title").html('<strong>Edición del pedido <u>' + PedidoId + '</u></strong>'); /* Actualiza el titulo */

	/* Actualiza los resultados de la tabla del detalle pedidos*/
	ConsultaDetallesDelPedido();

	/* Actualiza la bandeja principal con el nuevo pedido */
	ConsultaGrid();
}

function EnvioGuardado() {
	var modelo = {
		CatProductoId: $("#SelectProducto option:selected").val(),
		CatEstadoRepublicaId: $("#SelectEstadosRepublica option:selected").val(),
		PedidoId: parseInt($("#PedidoId").val()),
		Cantidad: parseInt($("#PedidoCantidad").val())
	}

	envioGenericos("/ConsultaInformacion/GuardadoPedido", modelo, MuestraResultado);
}

function EditarProducto(catProductoId, cantidad) {
	$("#SelectProducto").val(catProductoId);
	$("#PedidoCantidad").val(cantidad);

	var imagen = $("#SelectProducto option:selected").text();
	$("#ImagenProducto").attr("src", "/images/" + imagen + ".jpg");
}

function EliminarPedidoDetalle(jsonResult) {
	/* Se refresca la tabla */
	ConsultaDetallesDelPedido();

	if (parseInt(jsonResult) == 0) {
		MensajeExito("Se ha eliminado el pedido: " + $("#PedidoId").val())

		/* Oculta el modal de la edición */
		$(".swal2-backdrop-show").hide();

		/* Se refresca la bandeja principal */
		ConsultaGrid();
	}
}

function EliminarProducto(detallePedidoId) {
	var modelo = { detallePedidoId: detallePedidoId }

	envioGenericos("/ConsultaInformacion/EliminarPedidoDetalle", modelo, EliminarPedidoDetalle);
}