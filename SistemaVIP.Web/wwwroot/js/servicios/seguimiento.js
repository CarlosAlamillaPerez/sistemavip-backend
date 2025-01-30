$(document).ready(function () {
    // Variables globales
    let tablaSeguimiento = null;
    let filtrosActuales = {
        estado: '',
        presentadorId: '',
        fechaInicio: '',
        fechaFin: ''
    };

    window.accionesEvents = {
        'click .btn-detalle-servicio': function (e, value, row, index) {
            mostrarDetalleServicio(row.id);
        },
        'click .btn-cambiar-estado': function (e, value, row, index) {
            mostrarFormularioEstado(row.id, row.estado);
        }
    };

    // Inicialización
    inicializarTabla_();
    cargarPresentadores();
    inicializarFiltros();

    // Eventos de filtros
    $('#filtro-estado').on('change', aplicarFiltros);
    $('#filtro-presentador').on('change', aplicarFiltros);
    $('#fecha-desde, #fecha-hasta').on('change', validarFechas);

    // Eventos de acciones
    $(document).on('click', '.btn-detalle-servicio', function () {
        const id = $(this).data('id');
        mostrarDetalleServicio(id);
    });

    $(document).on('click', '.btn-cambiar-estado', function () {
        const id = $(this).data('id');
        const estadoActual = $(this).data('estado');
        mostrarFormularioEstado(id, estadoActual);
    });
});

function inicializarTabla_() {
    $('#tablaServicios').bootstrapTable({
        pagination: true,
        search: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        locale: 'es-MX',
        columns: [{
            field: 'id',
            title: 'ID',
            sortable: true
        }, {
            field: 'fechaServicio',
            title: 'Fecha',
            sortable: true,
            formatter: formatearFecha
        }, {
            field: 'presentador',
            title: 'Presentador',
            sortable: true
        }, {
            field: 'terapeuta',
            title: 'Terapeuta',
            sortable: true
        }, {
            field: 'tipoUbicacion',
            title: 'Ubicación',
            formatter: formatearUbicacion
        }, {
            field: 'montoTotal',
            title: 'Monto',
            formatter: formatearMonto,
            sortable: true
        }, {
            field: 'estado',
            title: 'Estado',
            formatter: formatearEstado,
            sortable: true
        }, {
            field: 'acciones',
            title: 'Acciones',
            formatter: formatearAcciones,
            events: 'accionesEvents'
        }]
    });
}

function cargarPresentadores() {
    $.ajax({
        url: '/Personal/ObtenerPresentadores',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const select = $('#filtro-presentador');
                select.empty();
                select.append('<option value="">Todos</option>');

                response.data.forEach(presentador => {
                    select.append(`<option value="${presentador.id}">
                        ${presentador.nombre} ${presentador.apellido}
                    </option>`);
                });
            }
        }
    });
}

function inicializarFiltros() {
    // Inicializar fechas con el mes actual
    const hoy = moment();
    const inicioMes = moment().startOf('month');

    $('#fecha-desde').val(inicioMes.format('YYYY-MM-DD'));
    $('#fecha-hasta').val(hoy.format('YYYY-MM-DD'));

    // Aplicar filtros iniciales
    aplicarFiltros();
}

function aplicarFiltros() {
    filtrosActuales = {
        estado: $('#filtro-estado').val(),
        presentadorId: $('#filtro-presentador').val(),
        fechaInicio: $('#fecha-desde').val(),
        fechaFin: $('#fecha-hasta').val()
    };

    cargarServicios();
}

function cargarServicios() {
    $.ajax({
        url: '/Servicios/ObtenerServiciosPorFiltro',
        type: 'GET',
        data: filtrosActuales,
        success: function (response) {
            if (response.success) {
                $('#tablaServicios').bootstrapTable('load', response.data);
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

function validarFechas() {
    const fechaInicio = moment($('#fecha-desde').val());
    const fechaFin = moment($('#fecha-hasta').val());

    if (fechaFin.isBefore(fechaInicio)) {
        window.alertService.error('Error', 'La fecha final debe ser mayor a la fecha inicial');
        return false;
    }

    // Limitar a 31 días
    const diferencia = fechaFin.diff(fechaInicio, 'days');
    if (diferencia > 31) {
        window.alertService.error('Error', 'El rango máximo permitido es de 31 días');
        return false;
    }

    aplicarFiltros();
    return true;
}

// Funciones de formateo
function formatearFecha(value) {
    return moment(value).format('DD/MM/YYYY HH:mm');
}

function formatearMonto(value) {
    return `$${parseFloat(value).toLocaleString('es-MX', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    })}`;
}

function formatearUbicacion(value) {
    const iconos = {
        'CONSULTORIO': '<i class="fas fa-hospital"></i>',
        'DOMICILIO': '<i class="fas fa-home"></i>'
    };
    return `${iconos[value] || ''} ${value}`;
}

function formatearEstado(value) {
    const estados = {
        'PENDIENTE': 'secondary',
        'POR_CONFIRMAR': 'info',
        'EN_PROCESO': 'primary',
        'FINALIZADO': 'success',
        'PAGADO': 'success',
        'LIQUIDADO': 'success',
        'CANCELADO': 'danger'
    };

    return `<span class="badge bg-${estados[value] || 'secondary'}">
        ${value.replace('_', ' ')}
    </span>`;
}

function formatearAcciones(value, row) {
    const acciones = [];

    // Botón de detalle siempre visible
    acciones.push(`
        <button class="btn btn-sm btn-info btn-detalle-servicio" 
                data-id="${row.id}" 
                title="Ver detalle">
            <i class="fas fa-eye"></i>
        </button>
    `);

    // Botón de cambio de estado según el estado actual
    if (row.estado === 'POR_CONFIRMAR') {
        acciones.push(`
            <button class="btn btn-sm btn-success btn-cambiar-estado" 
                    data-id="${row.id}" 
                    data-estado="${row.estado}"
                    title="Verificar pago">
                <i class="fas fa-check"></i>
                <span class="d-none d-md-inline ms-1">Verificar Pago</span>
            </button>
        `);
    } else if (row.estado === 'PAGADO') {
        acciones.push(`
            <button class="btn btn-sm btn-primary btn-cambiar-estado" 
                    data-id="${row.id}" 
                    data-estado="${row.estado}"
                    title="Liquidar comisiones">
                <i class="fas fa-dollar-sign"></i>
                <span class="d-none d-md-inline ms-1">Liquidar</span>
            </button>
            <button class="btn btn-sm btn-secondary btn-imprimir-liquidacion"
                    data-id="${row.id}"
                    title="Imprimir liquidación">
                <i class="fas fa-print"></i>
            </button>
        `);
    }

    return acciones.join(' ');
}

// Funciones de gestión de estados
function mostrarDetalleServicio(id) {
    $.ajax({
        url: `/Servicios/ObtenerDetalleServicio/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: 'Detalle del Servicio',
                    html: response,
                    width: '800px',
                    showCloseButton: true,
                    showConfirmButton: false
                });
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

function mostrarFormularioEstado(id, estadoActual) {
    const nuevoEstado = estadoActual === 'POR_CONFIRMAR' ? 'PAGADO' : 'LIQUIDADO';

    $.ajax({
        url: '/Servicios/ObtenerFormularioEstado',
        type: 'GET',
        data: {
            id: id,
            estadoActual: estadoActual,
            nuevoEstado: nuevoEstado
        },
        success: function (response) {
            Swal.fire({
                title: `Cambiar Estado a ${nuevoEstado.replace('_', ' ')}`,
                html: response,
                width: '600px',
                showCloseButton: true,
                showConfirmButton: false
            });

            // Inicializar validación del formulario
            inicializarValidacionFormulario();
        }
    });
}

function inicializarValidacionFormulario() {
    $('#formCambioEstado').validate({
        submitHandler: function (form) {
            guardarCambioEstado($(form));
        }
    });
}

function guardarCambioEstado(form) {
    const data = new FormData(form[0]);

    $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: data,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                Swal.close();
                window.alertService.successWithTimer(
                    'Éxito',
                    response.message,
                    1500,
                    () => aplicarFiltros()
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

function validarCambioEstado(servicioId, estadoActual, nuevoEstado) {
    // Validaciones para cambio a PAGADO
    if (nuevoEstado === 'PAGADO') {
        // Verificar que todos los comprobantes estén cargados
        let montoComprobantes = calcularTotalComprobantes();
        if (montoComprobantes < montoTotal) {
            return {
                valido: false,
                mensaje: 'El monto de los comprobantes no cubre el total del servicio'
            };
        }
    }

    // Validaciones para cambio a LIQUIDADO
    if (nuevoEstado === 'LIQUIDADO') {
        // Verificar que las comisiones estén calculadas
        if (!comisionesCalculadas) {
            return {
                valido: false,
                mensaje: 'Debe calcular las comisiones antes de liquidar'
            };
        }
    }

    return { valido: true };
}