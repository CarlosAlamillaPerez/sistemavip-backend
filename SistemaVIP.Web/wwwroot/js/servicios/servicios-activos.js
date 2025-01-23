$(document).ready(function () {
    let timer = null;
    const INTERVALO_ACTUALIZACION = 30000; // 30 segundos

    // Inicialización
    cargarServiciosActivos();
    iniciarActualizacionAutomatica();

    // Evento para actualización manual
    $(document).on('click', '#btn-refrescar', function () {
        cargarServiciosActivos();
    });

    // Evento para ver detalle del servicio
    $(document).on('click', '.btn-detalle-servicio', function () {
        const id = $(this).data('id');
        obtenerDetalleServicio(id);
    });

    // Control de tiempos
    function actualizarTiempos() {
        $('.control-tiempo').each(function () {
            const servicioId = $(this).data('servicio-id');
            const horaInicio = moment($(this).data('hora-inicio'));
            const duracionHoras = $(this).data('duracion');

            const ahora = moment();
            const tiempoTranscurrido = moment.duration(ahora.diff(horaInicio));
            const tiempoTotal = moment.duration(duracionHoras, 'hours');
            const tiempoRestante = moment.duration(tiempoTotal - tiempoTranscurrido);

            // Actualizar contadores
            $(`#tiempo-transcurrido-${servicioId}`).text(formatearTiempo(tiempoTranscurrido));
            $(`#tiempo-restante-${servicioId}`).text(formatearTiempo(tiempoRestante));

            // Calcular porcentaje y actualizar barra de progreso
            const porcentaje = (tiempoTranscurrido / tiempoTotal) * 100;
            const $progressBar = $(`#progress-bar-${servicioId}`);

            $progressBar.css('width', `${porcentaje}%`);
            $progressBar.text(`${Math.round(porcentaje)}%`);

            // Actualizar colores según el tiempo restante
            if (tiempoRestante.asMinutes() <= 15) {
                $progressBar.removeClass('bg-success bg-info').addClass('bg-warning');
            } else if (tiempoRestante.asMinutes() <= 0) {
                $progressBar.removeClass('bg-success bg-warning').addClass('bg-danger');
            }
        });
    }

    function cargarServiciosActivos() {
        $.ajax({
            url: '/Servicios/ObtenerServiciosActivos',
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    actualizarTablaServicios(response.data);
                    actualizarContadores(response.data);
                    actualizarTiempos();
                } else {
                    window.alertService.error('Error', response.message);
                }
            }
        });
    }

    function actualizarTablaServicios(servicios) {
        const $tabla = $('#tablaServiciosActivos');
        $tabla.bootstrapTable('load', servicios);
    }

    function actualizarContadores(servicios) {
        const activos = servicios.filter(s => s.estado === 'EN_PROCESO').length;
        const porFinalizar = servicios.filter(s =>
            s.estado === 'EN_PROCESO' &&
            moment(s.horaFin).diff(moment(), 'minutes') <= 15
        ).length;
        const finalizadosHoy = servicios.filter(s =>
            s.estado === 'FINALIZADO' &&
            moment(s.horaFin).isSame(moment(), 'day')
        ).length;
        const montoTotal = servicios
            .filter(s => moment(s.fechaServicio).isSame(moment(), 'day'))
            .reduce((sum, s) => sum + s.montoTotal, 0);

        $('#contador-activos').text(activos);
        $('#contador-finalizando').text(porFinalizar);
        $('#contador-finalizados').text(finalizadosHoy);
        $('#monto-total').text(`$${montoTotal.toLocaleString()}`);
    }

    function obtenerDetalleServicio(id) {
        $.ajax({
            url: `/Servicios/ObtenerServicio/${id}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    mostrarModalDetalle(response.data);
                } else {
                    window.alertService.error('Error', response.message);
                }
            }
        });
    }

    function mostrarModalDetalle(servicio) {
        Swal.fire({
            title: `Servicio #${servicio.id}`,
            html: construirHtmlDetalle(servicio),
            width: '800px',
            showCloseButton: true,
            showConfirmButton: false
        });
    }

    function iniciarActualizacionAutomatica() {
        if (timer) clearInterval(timer);
        timer = setInterval(function () {
            cargarServiciosActivos();
        }, INTERVALO_ACTUALIZACION);
    }

    function formatearTiempo(duration) {
        return `${String(Math.floor(duration.asHours())).padStart(2, '0')}:` +
            `${String(duration.minutes()).padStart(2, '0')}:` +
            `${String(duration.seconds()).padStart(2, '0')}`;
    }
});