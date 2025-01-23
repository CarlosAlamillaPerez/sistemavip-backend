$(document).ready(function () {

    // Eventos directos para los botones
    $(document).on('click', '#btn-detalle-presentador', function () {
        const id = $(this).data('id');
        obtenerDetallePresentador(id);
    });


    $(document).on('click', '#btn-editar-presentador', function () {
        const id = $(this).data('id');
        obtenerFormularioPresentador(id);
    });

    // Evento submit del formulario con validación
    $(document).on('submit', '#formPresentador', function (e) {
        e.preventDefault();

        // Validar el formulario
        if ($(this).valid()) {  // Esto usará las validaciones de ASP.NET MVC
            console.log("Formulario válido, procediendo a guardar");
            const id = $(this).data('id');
            guardarPresentador(id, $(this));
        } else {
            console.log("Formulario inválido");
            // Mostrar los errores de validación en el modal
            let errorMessage = "Por favor, complete todos los campos requeridos correctamente.";
            Swal.fire({
                icon: 'error',
                title: 'Error de Validación',
                text: errorMessage,
                showConfirmButton: true
            });
        }
    });

    // Inicializar la validación cuando el formulario se carga en el modal
    $(document).on('shown.bs.modal', function () {
        $.validator.unobtrusive.parse("#formPresentador");
    });

    $(document).on('click', '.btn-estado-presentador', function () {
        const id = $(this).data('id');
        const estadoActual = $(this).closest('tr').find('.badge').text().trim();

        // Lista de estados disponibles excluyendo el actual
        const estados = ['ACTIVO', 'INACTIVO', 'SUSPENDIDO'].filter(e => e !== estadoActual);

        // Creamos las opciones del select
        const opcionesEstado = estados.map(estado =>
            `<option value="${estado}">${estado}</option>`
        ).join('');

        Swal.fire({
            title: 'Cambiar Estado',
            html: `
            <div class="mb-3">
                <label class="form-label">Seleccione nuevo estado:</label>
                <select id="nuevoEstado" class="form-select">
                    ${opcionesEstado}
                </select>
            </div>
            <div class="mb-3">
                <label class="form-label">Motivo del cambio:</label>
                <textarea id="motivoEstado" class="form-control" rows="3" required></textarea>
            </div>`,
            showCancelButton: true,
            confirmButtonText: 'Cambiar',
            cancelButtonText: 'Cancelar',
            preConfirm: () => {
                const nuevoEstado = $('#nuevoEstado').val();
                const motivo = $('#motivoEstado').val();

                if (!motivo?.trim()) {
                    Swal.showValidationMessage('El motivo es requerido');
                    return false;
                }

                return { estado: nuevoEstado, motivoEstado: motivo };
            }
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Personal/CambiarEstadoPresentador?id=${id}&estado=${result.value.estado}&motivo=${result.value.motivo}`,
                    type: 'PATCH',
                    contentType: 'application/json',
                    success: function (response) {
                        if (response.success) {
                            window.alertService.successWithTimer('Éxito', response.message, 1500, () => {
                                window.location.reload();
                            });
                        } else {
                            window.alertService.error('Error', response.message);
                        }
                    }
                });
            }
        });
    });

});

function obtenerDetallePresentador(id) {
    $.ajax({
        url: '/Personal/ObtenerDetallePresentador',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            Swal.fire({
                title: 'Detalle del Presentador',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false
            });
        }
    });
}

function obtenerFormularioPresentador(id = null) {
    $.ajax({
        url: '/Personal/ObtenerFormularioPresentador',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            Swal.fire({
                title: id ? 'Editar Presentador' : 'Nuevo Presentador',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                didOpen: () => {
                    // Inicializar las validaciones cuando se abre el modal
                    $.validator.unobtrusive.parse("#formPresentador");
                }
            });
        }
    });
}

function guardarPresentador(id, form) {
    const presentadorId = form.attr('data-presentador-id');
    const formData = form.serializeArray();
    const data = {};

    formData.forEach(item => {
        data[item.name] = item.value;
    });

    $.ajax({
        url: presentadorId ? `/Personal/ActualizarPresentador/${presentadorId}` : '/Personal/GuardarPresentador',
        type: presentadorId ? 'PUT' : 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.close();
                window.alertService.successWithTimer('Éxito',response.message,1500,() => window.location.reload()
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
