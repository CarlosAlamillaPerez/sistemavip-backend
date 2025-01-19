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