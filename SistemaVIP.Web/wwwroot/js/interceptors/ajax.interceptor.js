// ajax.interceptor.js
$(document).ready(function () {
    // Contador de peticiones activas
    let activeRequests = 0;

    // Configuración global de Ajax
    $.ajaxSetup({
        beforeSend: function (xhr) {
            // Incrementar contador y mostrar loading si es la primera petición
            activeRequests++;
            if (activeRequests === 1) {
                window.loadingService.show();
            }

            // Agregar el header anti-forgery token si existe
            const token = $('input[name="__RequestVerificationToken"]').val();
            if (token) {
                xhr.setRequestHeader('RequestVerificationToken', token);
            }

            // Agregar header para identificar peticiones AJAX
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        },
        complete: function () {
            // Decrementar contador y ocultar loading si no hay más peticiones
            activeRequests--;
            if (activeRequests === 0) {
                window.loadingService.hide();
            }
        },
        error: function (xhr, status, error) {
            // Manejar errores
            let errorMessage = 'Ha ocurrido un error inesperado';

            try {
                const response = JSON.parse(xhr.responseText);
                errorMessage = response.message || errorMessage;
            } catch (e) {
                console.error('Error parsing error response:', e);
            }

            // Manejar diferentes códigos de estado
            switch (xhr.status) {
                case 401:
                    window.alertService.error('Sesión Expirada', 'Su sesión ha expirado, será redirigido al login.')
                        .then(() => window.location.href = '/Auth/Login');
                    break;
                case 403:
                    window.alertService.error('Acceso Denegado', 'No tiene permisos para realizar esta acción');
                    break;
                case 404:
                    window.alertService.error('No Encontrado', 'El recurso solicitado no existe');
                    break;
                case 422:
                    window.alertService.error('Error de Validación', errorMessage);
                    break;
                default:
                    window.alertService.error('Error', errorMessage);
            }
        }
    });

    // Interceptar los formularios que usen AJAX
    $(document).on('submit', 'form[data-ajax="true"]', function (e) {
        e.preventDefault();
        const $form = $(this);

        $.ajax({
            url: $form.attr('action'),
            type: $form.attr('method') || 'POST',
            data: new FormData($form[0]),
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    window.alertService.success('¡Éxito!', response.message);
                    // Disparar evento personalizado para manejo adicional
                    $form.trigger('ajax:success', [response]);
                } else {
                    window.alertService.error('Error', response.message);
                }
            }
        });
    });
});