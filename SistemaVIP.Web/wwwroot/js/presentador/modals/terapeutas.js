// comprobantes.js
$(document).ready(function () {
    // Variables globales
    let servicioId = null;
    let comprimiendo = false;

    // Inicializar eventos cuando se abre el modal
    $(document).on('shown.bs.modal', '#modalComprobantes', function () {
        servicioId = $(this).data('servicio-id');
        inicializarFormulario();
    });

    // Cambio en origen de pago
    $(document).on('change', '[name="OrigenPago"]', function () {
        const origen = $(this).val();
        if (origen === 'COMISION_TERAPEUTA') {
            $('[name="TipoComprobante"]').val('TRANSFERENCIA').prop('disabled', true);
            $('#camposTransferencia').slideDown();
            $('[name="NumeroOperacion"], [name="Comprobante"]').prop('required', true);
        } else {
            $('[name="TipoComprobante"]').prop('disabled', false);
            toggleCamposTransferencia($('[name="TipoComprobante"]').val() === 'TRANSFERENCIA');
        }
    });

    // Cambio en tipo de comprobante
    $(document).on('change', '[name="TipoComprobante"]', function () {
        toggleCamposTransferencia($(this).val() === 'TRANSFERENCIA');
    });

    // Manejo de eliminación individual
    $(document).on('click', '.btn-eliminar-comprobante', function () {
        const comprobanteId = $(this).data('id');
        eliminarComprobante(comprobanteId);
    });

    // Manejo de corrección (eliminación masiva)
    $(document).on('click', '#btnCorregir', function () {
        confirmarCorreccion();
    });

    // Manejo del formulario
    $(document).on('submit', '#formComprobante', function (e) {
        e.preventDefault();
        if (!comprimiendo && validarFormulario()) {
            guardarComprobante($(this));
        }
    });
});

function inicializarFormulario() {
    const $form = $('#formComprobante');
    $form.get(0).reset();
    $form.removeClass('was-validated');
    toggleCamposTransferencia(false);
}

function toggleCamposTransferencia(mostrar) {
    const $campos = $('#camposTransferencia');
    const $inputs = $('[name="NumeroOperacion"], [name="Comprobante"]');

    if (mostrar) {
        $campos.slideDown();
        $inputs.prop('required', true);
    } else {
        $campos.slideUp();
        $inputs.prop('required', false);
    }
}

function validarFormulario() {
    const $form = $('#formComprobante');

    if (!$form[0].checkValidity()) {
        $form.addClass('was-validated');
        return false;
    }

    return true;
}

async function comprimirImagen(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (event) {
            const img = new Image();
            img.src = event.target.result;
            img.onload = function () {
                const canvas = document.createElement('canvas');
                const ctx = canvas.getContext('2d');

                // Calcular nuevas dimensiones manteniendo aspecto
                let width = img.width;
                let height = img.height;
                if (width > 1920) {
                    height = height * (1920 / width);
                    width = 1920;
                }

                canvas.width = width;
                canvas.height = height;
                ctx.drawImage(img, 0, 0, width, height);

                // Convertir a JPG con calidad 0.7
                canvas.toBlob(function (blob) {
                    resolve(blob);
                }, 'image/jpeg', 0.7);
            };
            img.onerror = reject;
        };
        reader.onerror = reject;
    });
}

async function guardarComprobante($form) {
    try {
        comprimiendo = true;
        const formData = new FormData($form[0]);

        // Si hay archivo y es transferencia, comprimir
        const file = formData.get('Comprobante');
        if (file && file.size > 0) {
            const blob = await comprimirImagen(file);
            formData.set('Comprobante', blob, `Comprobante_${servicioId}_${moment().format('YYYYMMDDHHmmss')}.jpg`);
        }

        // Preparar datos para el API
        const data = {
            tipoComprobante: formData.get('TipoComprobante'),
            origenPago: formData.get('OrigenPago'),
            numeroOperacion: formData.get('NumeroOperacion') || null,
            notasComprobante: formData.get('NotasComprobante') || null,
            monto: parseFloat(formData.get('Monto'))
        };

        // Si hay archivo, agregarlo al FormData
        if (file && file.size > 0) {
            const uploadFormData = new FormData();
            uploadFormData.append('file', formData.get('Comprobante'));

            // Subir archivo primero
            const uploadResponse = await $.ajax({
                url: `/Presentador/SubirComprobante/${servicioId}`,
                type: 'POST',
                data: uploadFormData,
                processData: false,
                contentType: false
            });

            if (!uploadResponse.success) {
                throw new Error('Error al subir el comprobante');
            }

            data.urlComprobante = uploadResponse.url;
        }

        // Guardar comprobante
        const response = await $.ajax({
            url: `/Presentador/GuardarComprobante/${servicioId}`,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });

        if (response.success) {
            window.alertService.successWithTimer(
                'Éxito',
                'Comprobante guardado correctamente',
                1500,
                () => {
                    // Recargar tabla de comprobantes
                    cargarComprobantes();
                    // Limpiar formulario
                    inicializarFormulario();
                }
            );
        } else {
            throw new Error(response.message);
        }
    } catch (error) {
        window.alertService.error('Error', error.message);
    } finally {
        comprimiendo = false;
    }
}

async function eliminarComprobante(comprobanteId) {
    window.alertService.confirm(
        'Eliminar Comprobante',
        '¿Está seguro de eliminar este comprobante?',
        async () => {
            try {
                const response = await $.ajax({
                    url: `/Presentador/EliminarComprobante?servicioId=${servicioId}&comprobanteId=${comprobanteId}`,
                    type: 'DELETE'
                });

                if (response.success) {
                    window.alertService.successWithTimer(
                        'Éxito',
                        'Comprobante eliminado correctamente',
                        1500,
                        () => {
                            cargarComprobantes();
                        }
                    );
                } else {
                    throw new Error(response.message);
                }
            } catch (error) {
                window.alertService.error('Error', error.message);
            }
        }
    );
}

function confirmarCorreccion() {
    window.alertService.confirm(
        'Corregir Comprobantes',
        'Esta acción eliminará todos los comprobantes y permitirá editar nuevamente los campos del servicio. ¿Desea continuar?',
        async () => {
            try {
                const comprobantes = await $.get(`/Presentador/ObtenerComprobantes/${servicioId}`);

                // Eliminar comprobantes uno por uno
                for (const comprobante of comprobantes) {
                    await $.ajax({
                        url: `/Presentador/EliminarComprobante?servicioId=${servicioId}&comprobanteId=${comprobante.id}`,
                        type: 'DELETE'
                    });
                }

                window.alertService.successWithTimer(
                    'Éxito',
                    'Todos los comprobantes han sido eliminados',
                    1500,
                    () => {
                        // Cerrar modal
                        $('#modalComprobantes').modal('hide');
                        // Actualizar tabla principal
                        $('#tablaServicios').bootstrapTable('refresh');
                    }
                );
            } catch (error) {
                window.alertService.error('Error', 'No se pudieron eliminar todos los comprobantes');
            }
        }
    );
}

function cargarComprobantes() {
    $.ajax({
        url: `/Presentador/ObtenerComprobantes/${servicioId}`,
        type: 'GET',
        success: function (response) {
            // Actualizar la tabla de comprobantes
            // Esta parte dependerá de cómo quieras actualizar la UI
            $('#modalComprobantes').find('.modal-body').html(response);
        }
    });
}