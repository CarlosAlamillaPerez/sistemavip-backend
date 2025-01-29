// nuevo-servicio.js
$(document).ready(function () {
    // Variables para control de estado
    let montoOriginal = 0;
    let montoTerapeutaOriginal = 0;

    // Inicializar eventos cuando se abre el modal
    $(document).on('shown.bs.modal', '#modalNuevoServicio', function () {
        inicializarFormulario();
    });

    // Manejar cambio de tipo de ubicación
    $(document).on('change', 'input[name="TipoUbicacion"]', function () {
        const esDomicilio = $(this).val() === 'DOMICILIO';
        toggleCamposDomicilio(esDomicilio);
    });

    // Manejar cambios en montos para validación en tiempo real
    $(document).on('input', '[name="MontoTotal"], #montoComision, [name="DuracionHoras"]', function () {
        calcularMontos();
    });

    // Manejo del formulario
    $(document).on('submit', '#formNuevoServicio', function (e) {
        e.preventDefault();
        if (validarFormulario()) {
            guardarServicio($(this));
        }
    });
});

function inicializarFormulario() {
    const $form = $('#formNuevoServicio');
    $form.get(0).reset();
    $form.removeClass('was-validated');
    cargarTerapeutasDisponibles();

    // Inicializar campos y estados
    toggleCamposDomicilio(false);
    $('#montoTerapeuta').val('');
    $('#indicadorComisionHora').text('');
}

function toggleCamposDomicilio(mostrar) {
    const $camposDomicilio = $('#camposDomicilio');
    const $direccion = $('[name="Direccion"]');
    if (mostrar) {
        $camposDomicilio.slideDown();
        $direccion.prop('required', true);
        $('#notaTransporte').removeClass('d-none').html(
            '<div class="alert alert-info">' +
            '<i class="fas fa-info-circle"></i> Recuerda solicitar al cliente el pago por el gasto de transporte, ' +
            'o en caso de ser $0, indicar en las \'Notas\' el motivo' +
            '</div>'
        );
    } else {
        $camposDomicilio.slideUp();
        $direccion.prop('required', false);
        $('#notaTransporte').addClass('d-none');
    }
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
                    if (terapeuta.estado === 'ACTIVO') {
                        select.append(`
                            <option value="${terapeuta.terapeutaId}" 
                                    data-ciudad="${terapeuta.documentoIdentidad}">
                                ${terapeuta.nombreCompleto} - ${terapeuta.documentoIdentidad}
                            </option>
                        `);
                    }
                });
            }
        }
    });
}

function calcularMontos() {
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
}

function validarFormulario() {
    const $form = $('#formNuevoServicio');

    if (!$form[0].checkValidity()) {
        $form.addClass('was-validated');
        return false;
    }

    const montoTotal = parseFloat($('[name="MontoTotal"]').val());
    const montoTerapeuta = parseFloat($('#montoTerapeuta').val());
    const horas = parseInt($('[name="DuracionHoras"]').val());
    const montoTerapeutaPorHora = montoTerapeuta / horas;

    // Validaciones de negocio
    const errores = [];

    if (montoTotal < 1500) {
        errores.push('El monto total debe ser mayor a $1,500');
    }

    if (montoTerapeutaPorHora < 1000) {
        errores.push(`El monto por hora para la terapeuta debe ser al menos $1,000 (actual: $${montoTerapeutaPorHora.toFixed(2)})`);
    }

    if ($('input[name="TipoUbicacion"]:checked').val() === 'DOMICILIO') {
        const gastosTransporte = parseFloat($('[name="GastosTransporte"]').val()) || 0;
        const notasTransporte = $('[name="NotasTransporte"]').val().trim();

        if (gastosTransporte === 0 && !notasTransporte) {
            errores.push('Debe indicar el motivo cuando los gastos de transporte son $0');
        }
    }

    if (errores.length > 0) {
        window.alertService.error('Error de Validación', errores.join('<br>'));
        return false;
    }

    return true;
}

function guardarServicio($form) {
    const formData = new FormData($form[0]);

    const data = {
        fechaServicio: formData.get('FechaServicio'),
        tipoUbicacion: formData.get('TipoUbicacion'),
        direccion: formData.get('Direccion'),
        montoTotal: parseFloat(formData.get('MontoTotal')),
        gastosTransporte: formData.get('TipoUbicacion') === 'DOMICILIO' ?
            parseFloat(formData.get('GastosTransporte')) : null,
        notasTransporte: formData.get('NotasTransporte'),
        terapeutas: [{
            terapeutaId: parseInt(formData.get('TerapeutaId')),
            montoTerapeuta: parseFloat($('#montoTerapeuta').val())
        }],
        notas: formData.get('Notas'),
        duracionHoras: parseInt(formData.get('DuracionHoras'))
    };

    $.ajax({
        url: '/Presentador/CrearServicio',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                window.alertService.successWithTimer(
                    'Éxito',
                    'Servicio creado correctamente',
                    1500,
                    () => {
                        // Cerrar modal
                        $('#modalNuevoServicio').modal('hide');
                        // Actualizar tabla de servicios
                        $('#tablaServicios').bootstrapTable('refresh');
                    }
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        },
        error: function (xhr) {
            window.alertService.error('Error', 'Hubo un problema al procesar la solicitud');
        }
    });
}