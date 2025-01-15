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
        e.preventDefault(); // Previene el envío por defecto del formulario

        if (form.valid()) {
            button.prop('disabled', true);
            spinner.removeClass('d-none');

            const formData = {
                Email: $('#Email').val(),
                Password: $('#Password').val(),
                RememberMe: $('#RememberMe').is(':checked') // Asegúrate de tener este checkbox si usas "recordar sesión"
            };

            fetch('/api/Auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData),
                credentials: 'include'
            })
                .then(response => {
                    // Intentar convertir a JSON incluso si la respuesta no es exitosa
                    return response.json().then(data => {
                        if (!response.ok) {
                            throw new Error(data.message || 'Error de inicio de sesión');
                        }
                        return data;
                    });
                })
                .then(data => {
                    // Inicio de sesión exitoso
                    window.location.href = '/Home/Index';
                })
                .catch(error => {
                    // Manejar errores
                    showLoginError(error.message);
                })
                .finally(() => {
                    button.prop('disabled', false);
                    spinner.addClass('d-none');
                });

        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error de validación',
                text: 'Por favor, complete todos los campos correctamente.'
            });
        }

    });

    // Función para mostrar errores del servidor
    window.showLoginError = function (message) {
        button.prop('disabled', false);
        spinner.addClass('d-none');

        Swal.fire({
            icon: 'error',
            title: 'Error de inicio de sesión',
            text: message || 'Por favor, verifica tus credenciales e intenta nuevamente.'
        });
    };
});