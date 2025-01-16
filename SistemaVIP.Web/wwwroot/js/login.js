$(document).ready(function () {
    const form = $('form');
    const button = $('#loginButton');
    const spinner = $('#loginSpinner');

    // Inicializar validación
    form.validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 6
            }
        },
        messages: {
            Email: {
                required: "El email es requerido",
                email: "Por favor ingrese un email válido"
            },
            Password: {
                required: "La contraseña es requerida",
                minlength: "La contraseña debe tener al menos 6 caracteres"
            }
        },
        errorClass: 'is-invalid',
        validClass: 'is-valid'
    });

    // Manejar el submit del formulario
    form.on('submit', function (e) {
        if (!form.valid()) {
            e.preventDefault();
            Swal.fire({
                icon: 'error',
                title: 'Error de validación',
                text: 'Por favor, complete todos los campos correctamente.'
            });
            return false;
        }

        button.prop('disabled', true);
        spinner.removeClass('d-none');

        // Permitir que el formulario se envíe normalmente al controller
        return true;
    });
});