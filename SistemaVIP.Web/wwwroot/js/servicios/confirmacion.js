﻿$(document).ready(function () {
    // Validar estado al cargar la página
    verificarEstadoServicio();
    $('#btnConfirmar').on('click', function (e) {
        e.preventDefault();
        confirmarServicio();
    });
});

async function confirmarServicio() {
    try {
        if (!navigator.geolocation) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Tu dispositivo no soporta geolocalización. Es necesario para confirmar el servicio.'
            });
            return;
        }

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
            linkConfirmacion: window.linkConfirmacion,
            latitud: position.coords.latitude,
            longitud: position.coords.longitude
        };

        const response = await $.ajax({
            url: '/api/Servicio/confirmar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });

        if (response.success) {
            Swal.fire({
                icon: 'success',
                title: 'Servicio Iniciado',
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                // Verificar estado nuevamente antes de cerrar
                verificarEstadoServicio();
            });
        }

    } catch (error) {
        let errorMessage = 'Por favor, permite el acceso a tu ubicación para iniciar el servicio';
        if (error.code === 1) {
            errorMessage = 'Has denegado el permiso de ubicación. Es necesario para iniciar el servicio.';
        }

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage
        });
    }
}


async function verificarEstadoServicio() {
    try {
        const linkConfirmacion = window.linkConfirmacion;
        const response = await $.ajax({
            url: `/Servicio/confirmar/${linkConfirmacion}`,
            type: 'GET'
        });

        // Si el servicio ya está en proceso, ocultar botón y mostrar mensaje
        if (response.estado === 'EN_PROCESO') {
            $('#btnConfirmar').hide();
            Swal.fire({
                icon: 'info',
                title: 'En Proceso',
                text: 'Este servicio ya fue confirmado y está en proceso.',
                showConfirmButton: true
            }).then(() => {
                window.close();
            });
        }
    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'No se pudo verificar el estado del servicio.'
        });
    }
}
