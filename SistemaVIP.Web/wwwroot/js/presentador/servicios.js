$(document).ready(function () {
    // Variables globales
    let tablaSolicitudes = null;
    let filtrosActuales = {
        estado: '',
        terapeutaId: '',
        fechaInicio: '',
        fechaFin: ''
    };

    // Inicialización
    inicializarTabla();
    cargarTerapeutas();
    inicializarFiltros();
    inicializarEventos();

    // Eventos directos para los botones principales
    $(document).on('click', '#btnNuevoServicio', mostrarModalNuevoServicio);
    $(document).on('click', '#btnVerTerapeutas', mostrarModalTerapeutas);

    // Eventos para acciones de la tabla
    $(document).on('click', '.btn-detalle-servicio', function () {
        const id = $(this).closest('button').data('id');
        mostrarDetalleServicio(id);
    });

    $(document).on('click', '.btn-editar-servicio', function () {
        const id = $(this).closest('button').data('id');
        editarServicio(id);
    });

    $(document).on('click', '.btn-comprobante', function () {
        const id = $(this).closest('button').data('id');
        subirComprobante(id);
    });

    $(document).on('click', '.btn-cancelar-servicio', function () {
        const id = $(this).closest('button').data('id');
        confirmarCancelacion(id);
    });
});

window.accionesEvents = {
    'click .btn-detalle-servicio': function (e, value, row, index) {
        mostrarDetalleServicio(row.id);
    },
    'click .btn-editar-servicio': function (e, value, row, index) {
        editarServicio(row.id);
    },
    'click .btn-comprobante': function (e, value, row, index) {
        subirComprobante(row.id);
    },
    'click .btn-cancelar-servicio': function (e, value, row, index) {
        confirmarCancelacion(row.id);
    }
};

function inicializarTabla() {
    $('#tablaServicios').bootstrapTable({
        url: '/Presentador/ObtenerServicios',
        method: 'GET',
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
            field: 'terapeuta',
            title: 'Terapeuta',
            sortable: true
        }, {
            field: 'tipoUbicacion',
            title: 'Ubicación',
            formatter: formatearUbicacion
        }, {
            field: 'duracion',
            title: 'Duración',
            formatter: formatearDuracion
        }, {
            field: 'montoTotal',
            title: 'Monto Total',
            formatter: formatearMonto,
            sortable: true
        }, {
            field: 'comision',
            title: 'Comisión',
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

    tablaSolicitudes = $('#tablaServicios');
}

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

function inicializarFiltros() {
    // Inicializar fechas con el mes actual
    const hoy = moment();
    const inicioMes = moment().startOf('month');

    $('#fecha-desde').val(inicioMes.format('YYYY-MM-DD'));
    $('#fecha-hasta').val(hoy.format('YYYY-MM-DD'));

    // Aplicar filtros iniciales
    aplicarFiltros();
}

function inicializarEventos() {
    // Eventos de filtros
    $('#filtro-estado, #filtro-terapeuta').on('change', aplicarFiltros);
    $('#fecha-desde, #fecha-hasta').on('change', validarFechas);
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

function formatearDuracion(value) {
    return `${value} hora${value > 1 ? 's' : ''}`;
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
        'CANCELADO': 'danger'
    };

    return `<span class="badge bg-${estados[value] || 'secondary'}">
        ${value.replace('_', ' ')}
    </span>`;
}

// Formatear acciones según estado
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

    // Botón de edición según estado
    if (['PENDIENTE', 'EN_PROCESO', 'FINALIZADO'].includes(row.estado)) {
        acciones.push(`
            <button class="btn btn-sm btn-primary btn-editar-servicio" 
                    data-id="${row.id}" 
                    title="Editar servicio">
                <i class="fas fa-edit"></i>
            </button>
        `);
    }

    // Botón de comprobante para estados específicos
    if (['FINALIZADO', 'POR_CONFIRMAR'].includes(row.estado)) {
        acciones.push(`
            <button class="btn btn-sm btn-success btn-comprobante" 
                    data-id="${row.id}" 
                    title="Subir comprobante">
                <i class="fas fa-receipt"></i>
            </button>
        `);
    }

    // Botón de cancelación solo para PENDIENTE
    if (row.estado === 'PENDIENTE') {
        acciones.push(`
            <button class="btn btn-sm btn-danger btn-cancelar-servicio" 
                    data-id="${row.id}" 
                    title="Cancelar servicio">
                <i class="fas fa-times"></i>
            </button>
        `);
    }

    return acciones.join(' ');
}

// Funciones de Modal
function mostrarModalNuevoServicio() {
    $.ajax({
        url: '/Presentador/ObtenerFormularioServicio',
        type: 'GET',
        success: function (response) {
            Swal.fire({
                title: 'Nuevo Servicio',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                didOpen: () => {
                    inicializarFormularioServicio();
                }
            });
        }
    });
}

function mostrarModalTerapeutas() {
    $.ajax({
        url: '/Presentador/ObtenerTerapeutas',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    title: 'Terapeutas Disponibles',
                    html: generarHTMLTerapeutas(response.data),
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

function mostrarDetalleServicio(id) {
    $.ajax({
        url: `/Presentador/ObtenerDetalleServicio/${id}`,
        type: 'GET',
        success: function (response) {
            Swal.fire({
                title: 'Detalle del Servicio',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false
            });
        }
    });
}

// Funciones de Acción
function editarServicio(id) {
    $.ajax({
        url: `/Presentador/ObtenerFormularioServicio/${id}`,
        type: 'GET',
        success: function (response) {
            Swal.fire({
                title: 'Editar Servicio',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                didOpen: () => {
                    inicializarFormularioServicio(true);
                }
            });
        }
    });
}

function subirComprobante(id) {
    $.ajax({
        url: `/Presentador/ObtenerFormularioComprobante/${id}`,
        type: 'GET',
        success: function (response) {
            Swal.fire({
                title: 'Subir Comprobante',
                html: response,
                width: '600px',
                showCloseButton: true,
                showConfirmButton: false,
                didOpen: () => {
                    inicializarFormularioComprobante();
                }
            });
        }
    });
}

function confirmarCancelacion(id) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: 'Esta acción no se puede deshacer',
        icon: 'warning',
        input: 'textarea',
        inputLabel: 'Motivo de cancelación',
        inputPlaceholder: 'Ingrese el motivo de la cancelación...',
        inputAttributes: {
            required: 'true'
        },
        showCancelButton: true,
        confirmButtonText: 'Sí, cancelar',
        cancelButtonText: 'No, volver',
        showLoaderOnConfirm: true,
        preConfirm: (motivo) => {
            if (!motivo) {
                Swal.showValidationMessage('El motivo es requerido');
                return false;
            }
            return cancelarServicio(id, motivo);
        }
    });
}

function cancelarServicio(id, motivo) {
    return $.ajax({
        url: `/Presentador/CancelarServicio/${id}`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ motivoCancelacion: motivo }),
        success: function (response) {
            if (response.success) {
                window.alertService.successWithTimer(
                    'Éxito',
                    'Servicio cancelado correctamente',
                    1500,
                    () => {
                        aplicarFiltros();
                    }
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

// Funciones de Filtrado
function aplicarFiltros() {
    const filtros = {
        estado: $('#filtro-estado').val(),
        terapeutaId: $('#filtro-terapeuta').val(),
        fechaInicio: $('#fecha-desde').val(),
        fechaFin: $('#fecha-hasta').val()
    };

    tablaSolicitudes.bootstrapTable('refresh', {
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

    aplicarFiltros();
    return true;
}

// Funciones auxiliares
function generarHTMLTerapeutas(terapeutas) {
    return `
        <div class="table-responsive">
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th>Ciudad</th>
                        <th>Teléfono</th>
                        <th>Estado</th>
                    </tr>
                </thead>
                <tbody>
                    ${terapeutas.map(t => `
                        <tr>
                            <td>${t.nombreCompleto}</td>
                            <td>${t.documentoIdentidad}</td>
                            <td>${t.telefono}</td>
                            <td>
                                <span class="badge bg-${t.estado === 'ACTIVO' ? 'success' : 'danger'}">
                                    ${t.estado}
                                </span>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
}

function inicializarFormularioServicio() {
    const form = $('#formNuevoServicio');

    // Cargar terapeutas disponibles
    cargarTerapeutasDisponibles();

    // Manejar cambio de tipo de ubicación
    $('input[name="TipoUbicacion"]').on('change', function () {
        const esDomicilio = $(this).val() === 'DOMICILIO';
        $('#camposDomicilio').toggle(esDomicilio);
        $('[name="Direccion"]').prop('required', esDomicilio);
    });

    // Calcular monto terapeuta y comisión por hora
    $('[name="MontoTotal"], [name="DuracionHoras"], #montoComision').on('input', function () {
        const montoTotal = parseFloat($('[name="MontoTotal"]').val()) || 0;
        const comision = parseFloat($('#montoComision').val()) || 0;
        const horas = parseInt($('[name="DuracionHoras"]').val()) || 1;

        // Calcular monto terapeuta
        const montoTerapeuta = montoTotal - comision;
        $('#montoTerapeuta').val(montoTerapeuta.toFixed(2));

        // Validar monto mínimo terapeuta por hora
        const montoTerapeutaPorHora = montoTerapeuta / horas;
        if (montoTerapeutaPorHora < 1000) {
            $('#alertaMontoTerapeuta').html(
                '<i class="fas fa-exclamation-triangle"></i> El monto por hora para la terapeuta no puede ser menor a $1,000'
            ).removeClass('d-none').addClass('text-danger');
        } else {
            $('#alertaMontoTerapeuta').addClass('d-none');
        }

        // Calcular y mostrar comisión por hora
        const comisionPorHora = comision / horas;
        $('#indicadorComisionHora')
            .text(`Comisión por hora: $${comisionPorHora.toFixed(2)}`)
            .removeClass('text-success text-danger')
            .addClass(comisionPorHora < 500 ? 'text-danger' : 'text-success');
    });

    // Mostrar/ocultar campos de domicilio
    $('input[name="TipoUbicacion"]').on('change', function () {
        const esDomicilio = $(this).val() === 'DOMICILIO';
        $('#camposDomicilio').toggle(esDomicilio);

        if (esDomicilio) {
            $('#notaTransporte').removeClass('d-none').html(
                '<div class="alert alert-info">' +
                '<i class="fas fa-info-circle"></i> Recuerda solicitar al cliente el pago por el gasto de transporte, ' +
                'o en caso de ser $0, indicar en las \'Notas\' el motivo' +
                '</div>'
            );
        } else {
            $('#notaTransporte').addClass('d-none');
        }
    });

    // Validación y envío del formulario
    form.on('submit', function (e) {
        e.preventDefault();
        if (this.checkValidity()) {
            guardarServicio($(this));
        }
        $(this).addClass('was-validated');
    });
}

function cargarTerapeutasDisponibles() {
    $.ajax({
        url: '/Presentador/ObtenerTerapeutas',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const select = $('select[name="TerapeutaId"]');
                select.empty();
                select.append('<option value="">Seleccione una terapeuta...</option>');

                response.data.forEach(terapeuta => {
                    // Solo mostrar terapeutas activas
                    if (terapeuta.estado === 'ACTIVO') {
                        select.append(`
                            <option value="${terapeuta.terapeutaId}" 
                                    data-ciudad="${terapeuta.documentoIdentidad}">
                                ${terapeuta.nombreCompleto} - ${terapeuta.documentoIdentidad}
                            </option>
                        `);
                    }
                });
            } else {
                window.alertService.error('Error', 'No se pudieron cargar las terapeutas');
            }
        }
    });
}

function guardarServicio(form) {
    const formData = new FormData(form[0]);
    const horas = parseInt(formData.get('DuracionHoras'));
    const montoTerapeuta = parseFloat($('#montoTerapeuta').val());
    const montoTerapeutaPorHora = montoTerapeuta / horas;

    const data = {
        // No necesitamos enviar PresentadorId, se obtiene del usuario logueado
        fechaServicio: formData.get('FechaServicio'),
        tipoUbicacion: formData.get('TipoUbicacion'),
        direccion: formData.get('Direccion'),
        montoTotal: parseFloat(formData.get('MontoTotal')),
        gastosTransporte: formData.get('TipoUbicacion') === 'DOMICILIO' ?
            parseFloat(formData.get('GastosTransporte')) : null,
        notasTransporte: formData.get('NotasTransporte'),
        terapeutas: [{
            terapeutaId: parseInt(formData.get('TerapeutaId')),
            montoTerapeuta: montoTerapeuta
        }],
        notas: formData.get('Notas'),
        duracionHoras: horas
    };

    // Validaciones
    const errores = [];

    if (montoTerapeutaPorHora < 1000) {
        errores.push(`El monto por hora para la terapeuta debe ser al menos $1,000 (actual: $${montoTerapeutaPorHora.toFixed(2)})`);
    }

    if (data.tipoUbicacion === 'DOMICILIO' &&
        data.gastosTransporte === 0 &&
        !data.notasTransporte) {
        errores.push('Debe indicar el motivo cuando los gastos de transporte son $0');
    }

    // Validar campos requeridos
    if (!data.TerapeutaId) {
        errores.push('Debe seleccionar una terapeuta');
    }

    if (!data.TipoUbicacion) {
        errores.push('Debe seleccionar el tipo de ubicación');
    }

    if (data.TipoUbicacion === 'DOMICILIO' && !data.Direccion) {
        errores.push('La dirección es requerida para servicios a domicilio');
    }

    if (!data.MontoTotal || data.MontoTotal < 1500) {
        errores.push('El monto total debe ser mayor a $1,500');
    }

    if (!data.DuracionHoras || data.DuracionHoras < 1 || data.DuracionHoras > 24) {
        errores.push('La duración debe estar entre 1 y 24 horas');
    }

    if (data.montoTotal < 1500) {
        errores.push('El monto total debe ser mayor a $1,500');
    }

    if (data.terapeutas[0].montoTerapeuta < 0) {
        errores.push('El monto para la terapeuta no puede ser negativo');
    }

    // Si hay errores, mostrarlos y detener el proceso
    if (errores.length > 0) {
        window.alertService.error('Error de Validación', errores.join('<br>'));
        return;
    }

    // Llamada a la API
    $.ajax({
        url: '/Presentador/CrearServicio',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.close(); // Cerrar el modal
                window.alertService.successWithTimer(
                    'Éxito',
                    'Servicio creado correctamente',
                    1500,
                    () => {
                        // Actualizar tabla y contadores
                        aplicarFiltros();
                        actualizarResumen();
                    }
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        },
        error: function (xhr) {
            let errorMessage = 'Error al crear el servicio';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            window.alertService.error('Error', errorMessage);
        }
    });
}

