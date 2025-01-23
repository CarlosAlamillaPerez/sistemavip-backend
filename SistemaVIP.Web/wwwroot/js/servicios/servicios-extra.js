// servicios-extra.js

$(document).ready(function () {
    // Variables globales
    let tablaCatalogo = null;
    let servicioTerapeutaIdActual = null;

    // Inicialización
    inicializarTabla();
    inicializarEventos();

    // Eventos para los botones principales
    $(document).on('click', '#btn-nuevo-servicio', function () {
        mostrarFormularioServicio();
    });

    $(document).on('click', '.btn-editar-servicio', function () {
        const servicioExtraId = $(this).data('id');
        mostrarFormularioServicio(servicioExtraId);
    });

    $(document).on('click', '.btn-detalle-servicio', function () {
        const servicioExtraId = $(this).data('id');
        mostrarDetalleServicio(servicioExtraId);
    });

    $(document).on('click', '.btn-eliminar-servicio', function () {
        const servicioExtraId = $(this).data('id');
        confirmarEliminacion(servicioExtraId);
    });

    // Evento para el filtro de estado
    $('#filtro-estado').on('change', function () {
        tablaCatalogo.bootstrapTable('refresh');
    });

    // Handler para el formulario
    $(document).on('submit', '#formServicioExtra', function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            guardarServicioExtra($(this));
        }
    });
});

// Función de inicialización de la tabla
function inicializarTabla() {
    tablaCatalogo = $('#tablaServiciosExtra').bootstrapTable({
        url: '/Servicios/ObtenerServiciosExtraCatalogo',
        pagination: true,
        search: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        locale: 'es-MX',
        columns: [{
            field: 'nombre',
            title: 'Nombre',
            sortable: true
        }, {
            field: 'descripcion',
            title: 'Descripción'
        }, {
            field: 'estado',
            title: 'Estado',
            formatter: formatearEstado
        }, {
            field: 'fechaCreacion',
            title: 'Fecha Creación',
            formatter: formatearFecha,
            sortable: true
        }, {
            field: 'acciones',
            title: 'Acciones',
            formatter: formatearAcciones,
            events: window.accionesEvents
        }]
    });
}

// Definición de eventos para los botones de acción
window.accionesEvents = {
    'click .btn-detalle-servicio': function (e, value, row, index) {
        mostrarDetalleServicio(row.id);
    },
    'click .btn-editar-servicio': function (e, value, row, index) {
        mostrarFormularioServicio(row.id);
    },
    'click .btn-eliminar-servicio': function (e, value, row, index) {
        confirmarEliminacion(row.id);
    }
};

/////////////////////////////////////////////////////////////////////////////////////////

// Métodos de formateo
function formatearEstado(value, row) {
    const badge = value ?
        '<span class="badge bg-success">Activo</span>' :
        '<span class="badge bg-danger">Inactivo</span>';
    return badge;
}

function formatearFecha(value) {
    return value ? moment(value).format('DD/MM/YYYY HH:mm') : '';
}

function formatearAcciones(value, row) {
    return `
        <button class="btn btn-sm btn-info btn-detalle-servicio" data-id="${row.id}" title="Ver detalle">
            <i class="fas fa-eye"></i>
        </button>
        <button class="btn btn-sm btn-primary btn-editar-servicio" data-id="${row.id}" title="Editar">
            <i class="fas fa-pen-to-square"></i>
        </button>
        <button class="btn btn-sm btn-danger btn-eliminar-servicio" data-id="${row.id}" title="Eliminar">
            <i class="fas fa-trash"></i>
        </button>
    `;
}

// Métodos CRUD y operaciones
function mostrarFormularioServicio(id = null) {
    const url = '/Servicios/ObtenerFormularioServicioExtra';
    const params = id ? { servicioExtraId: id } : {};

    $.get(url, params)
        .done(function (response) {
            Swal.fire({
                title: id ? 'Editar Servicio Extra' : 'Nuevo Servicio Extra',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                didOpen: () => {
                    $.validator.unobtrusive.parse("#formServicioExtra");
                }
            });
        })
        .fail(function (error) {
            window.alertService.error('Error', 'No se pudo cargar el formulario');
        });
}

function guardarServicioExtra(form) {
    const servicioExtraId = form.data('id');
    const data = form.serializeArray().reduce((obj, item) => {
        obj[item.name] = item.value;
        return obj;
    }, {});

    const url = servicioExtraId ?
        `/Servicios/ActualizarServicioExtra/${servicioExtraId}` :
        '/Servicios/AgregarServicioExtra';

    $.ajax({
        url: url,
        type: servicioExtraId ? 'PUT' : 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.close();
                window.alertService.successWithTimer(
                    'Éxito',
                    response.message,
                    1500,
                    () => {
                        tablaCatalogo.bootstrapTable('refresh');
                    }
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

function mostrarDetalleServicio(id) {
    $.get(`/Servicios/ObtenerDetalleServicioExtra/${id}`)
        .done(function (response) {
            Swal.fire({
                title: 'Detalle del Servicio Extra',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false
            });
        })
        .fail(function (error) {
            window.alertService.error('Error', 'No se pudo cargar el detalle del servicio');
        });
}

function confirmarEliminacion(id) {
    window.alertService.confirm(
        'Eliminar Servicio Extra',
        '¿Está seguro que desea eliminar este servicio extra?',
        () => eliminarServicioExtra(id)
    );
}

function eliminarServicioExtra(id) {
    $.ajax({
        url: `/Servicios/EliminarServicioExtra/${id}`,
        type: 'DELETE',
        success: function (response) {
            if (response.success) {
                window.alertService.successWithTimer(
                    'Éxito',
                    response.message,
                    1500,
                    () => {
                        tablaCatalogo.bootstrapTable('refresh');
                    }
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

// Funciones auxiliares
function inicializarEventos() {
    // Refrescar tabla cuando cambia el filtro de estado
    $('#filtro-estado').on('change', function () {
        tablaCatalogo.bootstrapTable('refresh', {
            query: {
                estado: $(this).val()
            }
        });
    });
}