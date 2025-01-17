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
    $('#btn-detalle-presentador').on('click', function () {
        const id = $(this).data('id');
        obtenerDetallePresentador(id);
    });

    $('#btn-editar-presentador').on('click', function () {
        console.log("Botron presionado");
        const id = $(this).data('id');
        obtenerFormularioPresentador(id);
    });

    // Evento submit del formulario
    $('#formPresentador').on('submit', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        guardarPresentador(id, $(this));
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
                showCloseButton: false,
                showConfirmButton: false
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
                $('#tablaPresentadores').bootstrapTable('refresh');
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