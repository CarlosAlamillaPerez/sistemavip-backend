$(document).ready(function () {
    // Variables globales
    let presentadorSeleccionadoId = null;

    // Evento de selección de presentador
    $(document).on('click', '#lista-presentadores .list-group-item', function (e) {
        e.preventDefault();

        // Actualizar selección visual
        $('#lista-presentadores .list-group-item').removeClass('active');
        $(this).addClass('active');

        // Obtener datos del presentador
        presentadorSeleccionadoId = $(this).data('presentador-id');
        cargarTerapeutasAsignadas(presentadorSeleccionadoId);

        // Mostrar panel de terapeutas
        $('#panel-sin-seleccion').addClass('d-none');
        $('#panel-terapeutas').removeClass('d-none');
    });

    // Evento para asignar nueva terapeuta
    $(document).on('click', '#btn-asignar-terapeuta', function () {
        if (!presentadorSeleccionadoId) return;
        mostrarModalAsignacion(presentadorSeleccionadoId);
    });

    // Evento para cambiar estado de asignación
    $(document).on('click', '.btn-estado-asignacion', function () {
        const terapeutaId = $(this).data('terapeuta-id');
        const estado = $(this).data('estado');
        cambiarEstadoAsignacion(presentadorSeleccionadoId, terapeutaId, estado);
    });

    // Evento para eliminar asignación
    $(document).on('click', '.btn-eliminar-asignacion', function () {
        const terapeutaId = $(this).data('terapeuta-id');
        eliminarAsignacion(presentadorSeleccionadoId, terapeutaId);
    });
});

function cargarTerapeutasAsignadas(presentadorId) {
    $.ajax({
        url: '/Personal/ObtenerTerapeutasPorPresentador',
        type: 'GET',
        data: { presentadorId: presentadorId },
        success: function (response) {
            if (response.success) {
                actualizarTablaTerapeutas(response.data);
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

function actualizarTablaTerapeutas(terapeutas) {
    const tbody = $('#tabla-terapeutas-asignadas tbody');
    tbody.empty();

    terapeutas.forEach(terapeuta => {
        tbody.append(`
            <tr>
                <td>${terapeuta.nombreCompleto}</td>
                <td>${terapeuta.telefono}</td>
                <td>
                    <span class="badge bg-${terapeuta.estado === 'ACTIVO' ? 'success' : 'danger'}">
                        ${terapeuta.estado}
                    </span>
                </td>
                <td>${moment(terapeuta.fechaAsignacion).format('DD/MM/YYYY')}</td>
                <td>
                    <button class="btn btn-sm btn-warning btn-estado-asignacion"
                            data-terapeuta-id="${terapeuta.terapeutaId}"
                            data-estado="${terapeuta.estado === 'ACTIVO' ? 'INACTIVO' : 'ACTIVO'}">
                        <i class="fas fa-exchange-alt"></i>
                    </button>
                    <button class="btn btn-sm btn-danger btn-eliminar-asignacion"
                            data-terapeuta-id="${terapeuta.terapeutaId}">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `);
    });
}

function mostrarModalAsignacion(presentadorId) {
    $.ajax({
        url: '/Personal/ObtenerTerapeutasDisponibles',
        type: 'GET',
        data: { presentadorId: presentadorId },
        success: function (response) {
            Swal.fire({
                title: 'Asignar Nueva Terapeuta',
                html: response,
                width: '800px',
                showCloseButton: true,
                showConfirmButton: false
            });
        }
    });
}

function asignarTerapeuta(presentadorId, terapeutaId) {
    $.ajax({
        url: '/Personal/AsignarTerapeuta',
        type: 'POST',
        data: JSON.stringify({
            presentadorId: presentadorId,
            terapeutaId: terapeutaId
        }),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                Swal.close();
                window.alertService.successWithTimer('Éxito', response.message, 1500, () => {
                    cargarTerapeutasAsignadas(presentadorId);
                });
            } else {
                window.alertService.error('Error', response.message);
            }
        }
    });
}

function cambiarEstadoAsignacion(presentadorId, terapeutaId, nuevoEstado) {
    window.alertService.confirm(
        'Cambiar Estado',
        `¿Está seguro de cambiar el estado de la asignación a ${nuevoEstado}?`,
        () => {
            $.ajax({
                url: `/Personal/CambiarEstadoAsignacion/${presentadorId}/${terapeutaId}`,
                type: 'PATCH',
                data: JSON.stringify({ estado: nuevoEstado }),
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        window.alertService.successWithTimer('Éxito', response.message, 1500, () => {
                            cargarTerapeutasAsignadas(presentadorId);
                        });
                    } else {
                        window.alertService.error('Error', response.message);
                    }
                }
            });
        }
    );
}

function eliminarAsignacion(presentadorId, terapeutaId) {
    window.alertService.confirm(
        'Eliminar Asignación',
        '¿Está seguro de eliminar esta asignación?',
        () => {
            $.ajax({
                url: `/Personal/EliminarAsignacion/${presentadorId}/${terapeutaId}`,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        window.alertService.successWithTimer('Éxito', response.message, 1500, () => {
                            cargarTerapeutasAsignadas(presentadorId);
                        });
                    } else {
                        window.alertService.error('Error', response.message);
                    }
                }
            });
        }
    );
}

// Agregar al document.ready
$(document).on('click', '.btn-asignar', function () {
    const terapeutaId = $(this).data('terapeuta-id');
    asignarTerapeuta(presentadorSeleccionadoId, terapeutaId);
});