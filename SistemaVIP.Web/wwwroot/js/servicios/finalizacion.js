// finalizacion.js
$(document).ready(function () {
    // Validar estado al cargar la página
    verificarEstadoServicio();

    $('#btnFinalizar').on('click', function (e) {
        e.preventDefault();
        finalizarServicio();
    });
});

async function verificarEstadoServicio() {
    try {
        if (!navigator.geolocation) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Tu dispositivo no soporta geolocalización. Es necesario para finalizar el servicio.'
            });
            return;
        }

        const linkFinalizacion = window.linkFinalizacion;
        const response = await $.ajax({
            url: `/api/Servicio/finalizar/${linkFinalizacion}`,
            type: 'GET'
        });

        // Si el servicio ya está finalizado, ocultar botón y mostrar mensaje
        if (response.estado === 'FINALIZADO') {
            $('#btnFinalizar').hide();
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'No se pudo verificar el estado del servicio.'
        });
    }
}

async function finalizarServicio() {
    try {
        // Mostrar loading mientras obtenemos ubicación
        Swal.showLoading();

        const position = await new Promise((resolve, reject) => {
            navigator.geolocation.getCurrentPosition(resolve, reject, {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 0
            });
        });

        const data = {
            linkFinalizacion: window.linkFinalizacion,
            latitud: position.coords.latitude,
            longitud: position.coords.longitude
        };

        const response = await $.ajax({
            url: '/api/Servicio/finalizar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });

        if (response.success) {
            Swal.fire({
                icon: 'success',
                title: 'Servicio Finalizado',
                text: 'Gracias ♥ Bbe.',
                showConfirmButton: false,
                timer: 3000  // Cambio de 1500 a 3000
            }).then(() => {
                location.reload();
            });
        }

    } catch (error) {
        let errorMessage = 'Por favor, permite el acceso a tu ubicación para finalizar el servicio';
        if (error.code === 1) {
            errorMessage = 'Has denegado el permiso de ubicación. Es necesario para finalizar el servicio.';
        }

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage
        });
    }
}