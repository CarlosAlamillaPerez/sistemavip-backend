$(document).ready(function () {
    // Variables globales
    let presentadorSeleccionadoId = null;

    cargarMatrizAsignaciones();

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

    $(document).on('click', '.btn-asignar', function () {
        const terapeutaId = $(this).data('terapeuta-id');
        asignarTerapeuta(presentadorSeleccionadoId, terapeutaId);
    });

});

function cargarMatrizAsignaciones() {
    $.ajax({
        url: '/Personal/ObtenerMatrizAsignaciones',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                actualizarVistaAsignaciones(response.data);
            }
        },
        error: function (xhr) {
            window.alertService.error('Error', 'No se pudieron cargar las asignaciones');
        }
    });
}

function cargarTerapeutasAsignadas(presentadorId) {
    $.ajax({
        url: '/Personal/ObtenerTerapeutasPorPresentador',
        type: 'GET',
        data: { presentadorId: presentadorId },
        success: function (response) {
            if (response.success) {
                actualizarTablaTerapeutas(response.data || []);

                // Mostrar mensaje si no hay asignaciones
                if (!response.data?.length) {
                    $('#tabla-terapeutas-asignadas tbody')
                        .html('<tr><td colspan="5" class="text-center">No hay terapeutas asignadas</td></tr>');
                }
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
                    actualizarContadorTerapeutas(presentadorId);
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
                url: `/Personal/CambiarEstadoAsignacion`,
                type: 'PATCH',
                data: JSON.stringify({
                    presentadorId: presentadorId,
                    terapeutaId: terapeutaId,
                    estado: nuevoEstado
                }),
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
                url: `/Personal/EliminarAsignacion?presentadorId=${presentadorId}&terapeutaId=${terapeutaId}`,
                type: 'DELETE',
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        window.alertService.successWithTimer('Éxito', response.message, 1500, () => {
                            cargarTerapeutasAsignadas(presentadorId);
                            actualizarContadorTerapeutas(presentadorId);
                        });
                    } else {
                        window.alertService.error('Error', response.message);
                    }
                }
            });
        }
    );
}

function actualizarVistaAsignaciones(asignaciones) {
    const listaContainer = $('#lista-presentadores');
    listaContainer.empty(); // Limpiamos la lista actual

    if (!asignaciones || asignaciones.length === 0) {
        listaContainer.html(`
            <div class="alert alert-info">
                No hay presentadores disponibles
            </div>
        `);
        return;
    }

    // Agregamos cada presentador a la lista
    asignaciones.forEach(asignacion => {
        listaContainer.append(`
            <a href="#" class="list-group-item list-group-item-action ${asignacion.estado !== 'ACTIVO' ? 'disabled' : ''}"
               data-presentador-id="${asignacion.presentadorId}">
                <div class="d-flex w-100 justify-content-between">
                    <h6 class="mb-1">${asignacion.nombreCompleto}</h6>
                    <small>
                        <span class="badge bg-${asignacion.estado === 'ACTIVO' ? 'success' : 'danger'}">
                            ${asignacion.estado}
                        </span>
                        <span class="ms-2">${asignacion.cantidadTerapeutas} terapeutas</span>
                    </small>
                </div>
            </a>
        `);
    });
}

function actualizarContadorTerapeutas(presentadorId) {
    const presentadorElement = $(`#lista-presentadores .list-group-item[data-presentador-id="${presentadorId}"]`);
    const contadorElement = presentadorElement.find('small');

    // Obtener el estado del presentador
    const estadoElement = presentadorElement.find('.badge');
    const estado = estadoElement.text().trim();

    $.ajax({
        url: '/Personal/ObtenerTerapeutasPorPresentador',
        type: 'GET',
        data: { presentadorId: presentadorId },
        success: function (response) {
            if (response.success) {
                const cantidadTerapeutas = response.data.length;
                // Actualizar el contador manteniendo el badge de estado
                contadorElement.html(`
                    <span class="badge bg-${estado === 'ACTIVO' ? 'success' : 'danger'}">
                        ${estado}
                    </span>
                    <span class="ms-2">${cantidadTerapeutas} terapeutas</span>
                `);
            }
        }
    });
}
