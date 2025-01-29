// detalle-servicio.js
$(document).ready(function () {
    // Inicializar clipboard.js para el copiado de links
    const clipboard = new ClipboardJS('.btn-copy');

    // Evento de copiado exitoso
    clipboard.on('success', function (e) {
        // Cambiar temporalmente el ícono para dar feedback
        const $button = $(e.trigger);
        const $icon = $button.find('i');
        $icon.removeClass('fa-copy').addClass('fa-check');

        setTimeout(() => {
            $icon.removeClass('fa-check').addClass('fa-copy');
        }, 1500);

        e.clearSelection();
    });

    // Evento para eliminar comprobante
    $(document).on('click', '.btn-eliminar-comprobante', function () {
        const comprobanteId = $(this).data('id');
        const servicioId = $(this).closest('.modal').data('servicio-id');

        window.alertService.confirm(
            'Eliminar Comprobante',
            '¿Está seguro de eliminar este comprobante? Si elimina todos los comprobantes, podrá editar nuevamente los campos del servicio.',
            () => eliminarComprobante(servicioId, comprobanteId)
        );
    });
});

function eliminarComprobante(servicioId, comprobanteId) {
    $.ajax({
        url: `/Presentador/EliminarComprobante?servicioId=${servicioId}&comprobanteId=${comprobanteId}`,
        type: 'DELETE',
        success: function (response) {
            if (response.success) {
                window.alertService.successWithTimer(
                    'Éxito',
                    'Comprobante eliminado correctamente',
                    1500,
                    () => {
                        // Recargar el modal con la información actualizada
                        mostrarDetalleServicio(servicioId);
                        // Actualizar la tabla principal de servicios
                        $('#tablaServicios').bootstrapTable('refresh');
                    }
                );
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
                showConfirmButton: false,
                didOpen: () => {
                    // Guardar el ID del servicio en el modal para referencia
                    $('.modal').data('servicio-id', id);
                }
            });
        }
    });
}