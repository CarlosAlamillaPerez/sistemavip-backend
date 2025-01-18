$(document).ready(function () {
    $('#tablaPresentadores').bootstrapTable({
        onLoadSuccess: function (data) {
            console.log('Datos cargados:', data);
        },
        onLoadError: function (status, res) {
            console.error('Error al cargar datos:', status, res);
        }
    });

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
    const formData = form.serializeArray();
    const data = {};

    formData.forEach(item => {
        data[item.name] = item.value;
    });

    $.ajax({
        url: id ? `/Personal/ActualizarPresentador/${id}` : '/Personal/GuardarPresentador',
        type: id ? 'PUT' : 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.close();
                window.alertService.success('Éxito', response.message);
                window.location.reload();
            } else {
                window.alertService.error('Error', response.message);
            }
        },
        error: function (xhr) {
            window.alertService.error('Error', 'Hubo un problema al procesar la solicitud');
        }
    });
}

function queryParamsTerapeutas() {
    return {
        id: $('#presentadorId').val()
    };
}

function formatoFecha(value) {
    return value ? new Date(value).toLocaleDateString() : '';
}