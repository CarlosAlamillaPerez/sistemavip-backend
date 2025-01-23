$(document).ready(function () {
    // Eventos para los botones
    $(document).on('click', '#btn-detalle-terapeuta', function () {
        const id = $(this).data('id');
        obtenerDetalleTerapeuta(id);
    });

    $(document).on('click', '#btn-editar-terapeuta, #btn-nueva-terapeuta', function () {
        const id = $(this).data('id');
        obtenerFormularioTerapeuta(id);
    });

    // Evento submit del formulario con validación
    $(document).on('submit', '#formTerapeuta', function (e) {
        e.preventDefault();

        if ($(this).valid()) {
            console.log("Formulario válido, procediendo a guardar");
            const id = $(this).data('id');
            guardarTerapeuta(id, $(this));
        } else {
            console.log("Formulario inválido");
            window.alertService.error(
                'Error de Validación',
                'Por favor, complete todos los campos requeridos correctamente.'
            );
        }
    });

    // Inicializar validación en modal
    $(document).on('shown.bs.modal', function () {
        $.validator.unobtrusive.parse("#formTerapeuta");
    });

    $(document).on('click', '.btn-estado-terapeuta', function () {
        const id = $(this).data('id');
        const estadoActual = $(this).closest('tr').find('.badge').text().trim();

        const estados = ['ACTIVO', 'INACTIVO', 'SUSPENDIDO'].filter(e => e !== estadoActual);
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
                    url: `/Personal/CambiarEstadoTerapeuta?id=${id}&estado=${result.value.estado}&motivo=${result.value.motivo}`,
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

function obtenerDetalleTerapeuta(id) {
    $.ajax({
        url: '/Personal/ObtenerDetalleTerapeuta',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            Swal.fire({
                title: 'Detalle de la Terapeuta',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false
            });
        }
    });
}

function obtenerFormularioTerapeuta(id = null) {
    $.ajax({
        url: '/Personal/ObtenerFormularioTerapeuta',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            Swal.fire({
                title: id ? 'Editar Terapeuta' : 'Nueva Terapeuta',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false,
                didOpen: () => {
                    $.validator.unobtrusive.parse("#formTerapeuta");
                }
            });
        }
    });
}

function guardarTerapeuta(id, form) {
    const terapeutaId = form.attr('data-terapeuta-id');
    const formData = form.serializeArray();
    const data = {};

    formData.forEach(item => {
        data[item.name] = item.value;
    });

    $.ajax({
        url: terapeutaId ? `/Personal/ActualizarTerapeuta/${terapeutaId}` : '/Personal/GuardarTerapeuta',
        type: terapeutaId ? 'PUT' : 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.close();
                window.alertService.successWithTimer(
                    'Éxito',
                    response.message,
                    1500,
                    () => window.location.reload()
                );
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}