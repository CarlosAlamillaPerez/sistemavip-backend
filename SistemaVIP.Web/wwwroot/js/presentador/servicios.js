$(document).ready(function () {
    // Variables globales
    let tablaServicios = null;

    // Inicialización
    inicializarTabla_servicio();
    inicializarFiltros();
    inicializarEventos();

    $(document).on('click', '#btnNuevoServicio', function () {
        cargarModalNuevoServicio();
    });

    $(document).on('click', '#btnVerTerapeutas', function () {
        cargarModalTerapeutas();
    });

    // Event listeners para botones de la tabla
    $(document).on('click', '.btn-detalle-servicio', function () {
        const id = $(this).data('id');
        cargarModalDetalle(id);
    });

    $(document).on('click', '.btn-editar-servicio', function () {
        const id = $(this).data('id');
        cargarModalEditar(id);
    });

    $(document).on('click', '.btn-comprobante', function () {
        const id = $(this).data('id');
        cargarModalComprobantes(id);
    });

    $(document).on('click', '.btn-servicios-extra', function () {
        const id = $(this).data('id');
        cargarModalServiciosExtra(id);
    });

    $(document).on('click', '.btn-eliminar-servicio', function () {
        const id = $(this).data('id');
        confirmarEliminacionServicio(id);
    });

    $(document).on('click', '.btn-cancelar-servicio', function () {
        const id = $(this).data('id');
        mostrarModalCancelacion(id);
    });
});

// Funciones de inicialización
function inicializarTabla_servicio() {
    console.log('Inicializando tabla...');

    if (!$.fn.bootstrapTable) {
        console.error('Bootstrap Table no está inicializado');
        return;
    }

    const $tabla = $('#tablaServicios');
    if ($tabla.length === 0) {
        console.error('No se encontró el elemento tablaServicios');
        return;
    }

    // Inicializar la tabla primero con la configuración base
    tablaServicios = $tabla.bootstrapTable({
        data: [], // Inicialmente vacío
        pagination: true,
        search: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        locale: 'es-MX',
        showRefresh: true,
        sidePagination: 'client',
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
            field: 'terapeutas',
            title: 'Terapeuta',
            sortable: true,
            formatter: function (value) {
                return value && value.length > 0 ?
                    value[0].nombreTerapeuta : '';
            }
        }, {
            field: 'tipoUbicacion',
            title: 'Ubicación',
            formatter: formatearUbicacion
        }, {
            field: 'duracionHoras',
            title: 'Duración',
            formatter: (value) => `${value} hora(s)`
        }, {
            field: 'montoTotal',
            title: 'Monto Total',
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
            events: window.accionesEvents
        }]
    });

    // Función para cargar los datos
    function cargarDatos() {
        $.ajax({
            url: '/Presentador/ObtenerServicios',
            type: 'GET',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                console.log('Datos obtenidos:', response);
                if (response.success && response.data) {
                    tablaServicios.bootstrapTable('load', response.data);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error obteniendo datos:', error);
                window.alertService.error('Error', 'No se pudieron cargar los servicios');
            }
        });
    }

    // Cargar datos iniciales
    cargarDatos();

    // Agregar listener para refrescar
    $('#btn-refrescar').on('click', function () {
        cargarDatos();
    });
}

function inicializarFiltros() {
    const hoy = moment();
    const inicioMes = moment().startOf('month');

    $('#fecha-desde').val(inicioMes.format('YYYY-MM-DD'));
    $('#fecha-hasta').val(hoy.format('YYYY-MM-DD'));

    cargarTerapeutas();
    aplicarFiltros();
}

function inicializarEventos() {
    $('#filtro-estado, #filtro-terapeuta').on('change', aplicarFiltros);
    $('#fecha-desde, #fecha-hasta').on('change', validarFechas);
}

// Funciones de carga de datos
function cargarTerapeutas() {
    $.ajax({
        url: '/Presentador/ObtenerTerapeutas',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const select = $('#filtro-terapeuta');
                select.empty();
                select.append('<option value="">Todas</option>');
                response.data.forEach(terapeuta => {
                    select.append(`<option value="${terapeuta.terapeutaId}">
                        ${terapeuta.nombreCompleto}
                    </option>`);
                });
            }
        }
    });
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
                data-id="${row.id}" title="Ver detalle">
            <i class="fas fa-eye"></i>
        </button>
    `);

    if (row.estado === 'PENDIENTE') {
        acciones.push(`
        <button class="btn btn-sm btn-danger btn-eliminar-servicio" 
                data-id="${row.id}" title="Eliminar servicio">
            <i class="fas fa-trash"></i>
        </button>
        <button class="btn btn-sm btn-warning btn-cancelar-servicio" 
                data-id="${row.id}" title="Cancelar servicio">
            <i class="fas fa-ban"></i>
        </button>
    `);
    }

    // Botón de edición según estado
    if (['PENDIENTE', 'EN_PROCESO', 'FINALIZADO'].includes(row.estado)) {
        acciones.push(`
            <button class="btn btn-sm btn-primary btn-editar-servicio" 
                    data-id="${row.id}" title="Editar servicio">
                <i class="fas fa-edit"></i>
            </button>
        `);
    }

    // Botón de comprobante para estados específicos
    if (['FINALIZADO', 'POR_CONFIRMAR'].includes(row.estado)) {
        acciones.push(`
            <button class="btn btn-sm btn-success btn-comprobante" 
                    data-id="${row.id}" title="Comprobantes">
                <i class="fas fa-receipt"></i>
            </button>
        `);
    }

    // Botón de servicios extra
    if (['EN_PROCESO', 'FINALIZADO'].includes(row.estado)) {
        acciones.push(`
            <button class="btn btn-sm btn-warning btn-servicios-extra" 
                    data-id="${row.id}" title="Servicios Extra">
                <i class="fas fa-star"></i>
            </button>
        `);
    }

    return acciones.join(' ');
}

// Funciones de filtrado
function aplicarFiltros() {
    const filtros = {
        estado: $('#filtro-estado').val(),
        terapeutaId: $('#filtro-terapeuta').val(),
        fechaInicio: $('#fecha-desde').val(),
        fechaFin: $('#fecha-hasta').val()
    };

    tablaServicios.bootstrapTable('refresh', {
        query: filtros
    });
}

function validarFechas() {
    const fechaInicio = moment($('#fecha-desde').val());
    const fechaFin = moment($('#fecha-hasta').val());

    if (fechaFin.isBefore(fechaInicio)) {
        window.alertService.error('Error', 'La fecha final debe ser mayor a la fecha inicial');
        return false;
    }

    const diferencia = fechaFin.diff(fechaInicio, 'days');
    if (diferencia > 31) {
        window.alertService.error('Error', 'El rango máximo permitido es de 31 días');
        return false;
    }

    aplicarFiltros();
    return true;
}

// Funciones de carga de modales
function cargarModalNuevoServicio() {
    $.ajax({
        url: '/Presentador/ObtenerFormularioServicio',
        type: 'GET',
        success: function(response) {
            if (response.success === false) {
                window.alertService.error('Error', response.message);
                return;
            }
            // Si es exitoso, el response será el HTML de la vista parcial
            $('#modalSection').html(response);
            const modal = new bootstrap.Modal(document.getElementById('modalNuevoServicio'));
            modal.show();
        },
        error: function() {
            window.alertService.error('Error', 'No se pudo cargar el formulario');
        }
    });
}

function cargarModalTerapeutas() {
    $.ajax({
        url: '/Presentador/ObtenerModalTerapeutas',
        type: 'GET',
        success: function(response) {
            if (response.success === false) {
                window.alertService.error('Error', response.message);
                return;
            }
            // Si es exitoso, el response será el HTML de la vista parcial
            $('#modalSection').html(response);
            const modal = new bootstrap.Modal(document.getElementById('modalTerapeutas'));
            modal.show();
        },
        error: function() {
            window.alertService.error('Error', 'No se pudo cargar el listado de terapeutas');
        }
    });
}

function cargarModalDetalle(id) {
    $.ajax({
        url: `/Presentador/ObtenerDetalleServicio/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.success === false) {
                window.alertService.error('Error', response.message);
                return;
            }
            Swal.fire({
                title: `Detalle del Servicio #${id}`,
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                customClass: {
                    container: 'swal-servicio-container',
                    popup: 'swal-servicio-popup'
                }
            });
        },
        error: function () {
            window.alertService.error('Error', 'No se pudo cargar el detalle del servicio');
        }
    });
}

function cargarModalEditar(id) {
    $.ajax({
        url: `/Presentador/ObtenerFormularioServicio/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.success === false) {
                window.alertService.error('Error', response.message);
                return;
            }
            Swal.fire({
                title: 'Editar Servicio',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                customClass: {
                    container: 'swal-servicio-container',
                    popup: 'swal-servicio-popup'
                }
            });
        },
        error: function () {
            window.alertService.error('Error', 'No se pudo cargar el formulario de edición');
        }
    });
}

function cargarModalComprobantes(id) {
    $.ajax({
        url: `/Presentador/ObtenerFormularioComprobante/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.success === false) {
                window.alertService.error('Error', response.message);
                return;
            }
            Swal.fire({
                title: 'Comprobantes de Pago',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                customClass: {
                    container: 'swal-servicio-container',
                    popup: 'swal-servicio-popup'
                }
            });
        },
        error: function () {
            window.alertService.error('Error', 'No se pudo cargar el formulario de comprobantes');
        }
    });
}

function cargarModalServiciosExtra(id) {
    $.ajax({
        url: `/Presentador/ObtenerFormularioServiciosExtra/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.success === false) {
                window.alertService.error('Error', response.message);
                return;
            }
            Swal.fire({
                title: 'Servicios Extra',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                customClass: {
                    container: 'swal-servicio-container',
                    popup: 'swal-servicio-popup'
                }
            });
        },
        error: function () {
            window.alertService.error('Error', 'No se pudo cargar el formulario de servicios extra');
        }
    });
}

// Definición de eventos para la tabla
window.accionesEvents = {
    'click .btn-detalle-servicio': function (e, value, row) {
        cargarModalDetalle(row.id);
    },
    'click .btn-editar-servicio': function (e, value, row) {
        cargarModalEditar(row.id);
    },
    'click .btn-comprobante': function (e, value, row) {
        cargarModalComprobantes(row.id);
    },
    'click .btn-servicios-extra': function (e, value, row) {
        cargarModalServiciosExtra(row.id);
    }
};

function confirmarEliminacionServicio(id) {
    window.alertService.confirm(
        'Eliminar Servicio',
        '¿Está seguro de eliminar este servicio? Esta acción no se puede deshacer.',
        () => {
            $.ajax({
                url: `/Presentador/EliminarServicio/${id}`,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        window.alertService.successWithTimer(
                            'Éxito',
                            'Servicio eliminado correctamente',
                            1500,
                            () => tablaServicios.bootstrapTable('refresh')
                        );
                    } else {
                        window.alertService.error('Error', response.message);
                    }
                }
            });
        }
    );
}

function mostrarModalCancelacion(id) {
    Swal.fire({
        title: 'Cancelar Servicio',
        html: `
            <div class="form-group">
                <label class="form-label">Motivo de cancelación</label>
                <input type="text" id="motivoCancelacion" class="form-control" required>
            </div>
            <div class="form-group mt-3">
                <label class="form-label">Notas adicionales</label>
                <textarea id="notasCancelacion" class="form-control" rows="3"></textarea>
            </div>
            <div class="form-group mt-3">
                <label class="form-label">Monto comisión por cancelación</label>
                <input type="number" id="montoComisionCancelacion" class="form-control" value="0" min="0">
            </div>
        `,
        showCancelButton: true,
        confirmButtonText: 'Cancelar Servicio',
        cancelButtonText: 'Cerrar',
        preConfirm: () => {
            const motivo = document.getElementById('motivoCancelacion').value;
            if (!motivo) {
                Swal.showValidationMessage('El motivo de cancelación es requerido');
                return false;
            }
            return {
                motivoCancelacion: motivo,
                notasCancelacion: document.getElementById('notasCancelacion').value,
                montoComisionCancelacion: parseFloat(document.getElementById('montoComisionCancelacion').value) || 0
            };
        }
    }).then((result) => {
        if (result.isConfirmed) {
            cancelarServicio(id, result.value);
        }
    });
}

function cancelarServicio(id, datos) {
    $.ajax({
        url: `/Presentador/CancelarServicio/${id}`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(datos),
        success: function (response) {
            if (response.success) {
                window.alertService.successWithTimer(
                    'Éxito',
                    'Servicio cancelado correctamente',
                    1500,
                    () => tablaServicios.bootstrapTable('refresh')
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}