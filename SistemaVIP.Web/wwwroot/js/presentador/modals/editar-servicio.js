// editar-servicio.js
$(document).ready(function () {
    // Variables para control de estado
    let montoOriginal = 0;
    let montoTerapeutaOriginal = 0;

    // Inicializar eventos cuando se abre el modal
    $(document).on('shown.bs.modal', '#modalEditarServicio', function () {
        inicializarFormulario__();
    });

    // Manejar el envío del formulario
    $(document).on('submit', '#formEditarServicio', function (e) {
        e.preventDefault();
        if (validarFormulario()) {
            guardarCambios($(this));
        }
    });

    // Manejar cambio de tipo de ubicación
    $(document).on('change', 'input[name="TipoUbicacion"]', function () {
        const esDomicilio = $(this).val() === 'DOMICILIO';
        toggleCamposDomicilio(esDomicilio);
    });

    // Manejar cambios en montos para validación en tiempo real
    $(document).on('input', '[name="MontoTotal"], [name="MontoTerapeuta"]', function () {
        validarMontos();
    });
});

function inicializarFormulario__() {
    const $form = $('#formEditarServicio');

    // Guardar valores originales para comparación
    montoOriginal = parseFloat($('[name="MontoTotal"]').val());
    montoTerapeutaOriginal = parseFloat($('[name="MontoTerapeuta"]').val());

    // Inicializar validaciones de Bootstrap
    $form.get(0).classList.remove('was-validated');

    // Configurar campos según estado
    const estado = $form.data('estado');
    const tieneComprobantes = $form.data('tiene-comprobantes') === 'true';

    configurarCamposSegunEstado(estado, tieneComprobantes);
}

function configurarCamposSegunEstado(estado, tieneComprobantes) {
    // Objeto de configuración de campos según estado
    const configuracionCampos = {
        PENDIENTE: {
            tipoUbicacion: true,
            direccion: true,
            montoTotal: true,
            gastosTransporte: true,
            notasTransporte: true,
            montoTerapeuta: true,
            notas: true,
            duracionHoras: true
        },
        EN_PROCESO: {
            tipoUbicacion: false,
            direccion: false,
            montoTotal: !tieneComprobantes,
            gastosTransporte: false,
            notasTransporte: false,
            montoTerapeuta: !tieneComprobantes,
            notas: !tieneComprobantes,
            duracionHoras: !tieneComprobantes
        },
        FINALIZADO: {
            tipoUbicacion: false,
            direccion: false,
            montoTotal: !tieneComprobantes,
            gastosTransporte: false,
            notasTransporte: false,
            montoTerapeuta: !tieneComprobantes,
            notas: !tieneComprobantes,
            duracionHoras: !tieneComprobantes
        }
    };

    // Aplicar configuración
    const config = configuracionCampos[estado] || {};
    Object.entries(config).forEach(([campo, editable]) => {
        $(`[name="${campo}"]`).prop('disabled', !editable);
    });
}

function toggleCamposDomicilio(mostrar) {
    const $seccionDomicilio = $('#seccionDireccion, #seccionGastosTransporte');
    if (mostrar) {
        $seccionDomicilio.slideDown();
        $('[name="Direccion"]').prop('required', true);
    } else {
        $seccionDomicilio.slideUp();
        $('[name="Direccion"]').prop('required', false);
    }
}

function validarMontos() {
    const montoTotal = parseFloat($('[name="MontoTotal"]').val()) || 0;
    const montoTerapeuta = parseFloat($('[name="MontoTerapeuta"]').val()) || 0;
    const duracionHoras = parseInt($('[name="DuracionHoras"]').val()) || 1;

    let errores = [];

    // Validaciones de montos
    if (montoTotal < 1500) {
        errores.push('El monto total debe ser al menos $1,500');
    }

    if (montoTerapeuta / duracionHoras < 1000) {
        errores.push('El monto por hora para la terapeuta debe ser al menos $1,000');
    }

    if (montoTerapeuta >= montoTotal) {
        errores.push('El monto para la terapeuta no puede ser mayor o igual al monto total');
    }

    // Mostrar/ocultar errores
    const $alertaMontos = $('#alertaMontos');
    if (errores.length > 0) {
        if (!$alertaMontos.length) {
            $('<div id="alertaMontos" class="alert alert-danger mt-2"></div>')
                .insertAfter('[name="MontoTerapeuta"]');
        }
        $alertaMontos.html(errores.join('<br>'));
        return false;
    } else {
        $alertaMontos.remove();
        return true;
    }
}

function validarFormulario() {
    const $form = $('#formEditarServicio');

    if (!$form[0].checkValidity()) {
        $form[0].classList.add('was-validated');
        return false;
    }

    return validarMontos();
}

function guardarCambios($form) {
    const servicioId = $form.data('servicio-id');
    const formData = new FormData($form[0]);
    const data = {};

    // Convertir FormData a objeto
    formData.forEach((value, key) => {
        data[key] = value;
    });

    // Agregar campos específicos según tipo de ubicación
    if (data.TipoUbicacion !== 'DOMICILIO') {
        data.GastosTransporte = null;
        data.NotasTransporte = null;
        data.Direccion = null;
    }

    $.ajax({
        url: `/Presentador/ActualizarServicio/${servicioId}`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                window.alertService.successWithTimer(
                    'Éxito',
                    'Servicio actualizado correctamente',
                    1500,
                    () => {
                        // Cerrar modal
                        $('#modalEditarServicio').modal('hide');
                        // Actualizar tabla de servicios
                        $('#tablaServicios').bootstrapTable('refresh');
                    }
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        },
        error: function (xhr) {
            window.alertService.error(
                'Error',
                'No se pudo actualizar el servicio. Por favor, intente nuevamente.'
            );
        }
    });
}