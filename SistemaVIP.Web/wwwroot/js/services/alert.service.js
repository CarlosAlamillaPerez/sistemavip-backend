// alert.service.js
class AlertService {
    success(title, text, callback) {
        Swal.fire({
            icon: 'success',
            title,
            text,
            confirmButtonColor: '#198754'
        }).then(() => {
            if (callback) callback();
        });
    }

    error(title, text, callback) {
        Swal.fire({
            icon: 'error',
            title,
            text,
            confirmButtonColor: '#dc3545'
        }).then(() => {
            if (callback) callback();
        });
    }

    warning(title, text, callback) {
        Swal.fire({
            icon: 'warning',
            title,
            text,
            confirmButtonColor: '#ffc107'
        }).then(() => {
            if (callback) callback();
        });
    }

    info(title, text, callback) {
        Swal.fire({
            icon: 'info',
            title,
            text,
            confirmButtonColor: '#0dcaf0'
        }).then(() => {
            if (callback) callback();
        });
    }

    confirm(title, text, callback, options = {}) {
        Swal.fire({
            icon: 'warning',
            title,
            text,
            showCancelButton: true,
            confirmButtonColor: '#198754',
            cancelButtonColor: '#dc3545',
            confirmButtonText: options.confirmButtonText || 'Confirmar',
            cancelButtonText: options.cancelButtonText || 'Cancelar'
        }).then(result => {
            if (result.isConfirmed && callback) {
                callback();
            }
        });
    }

    confirmCambioEstado(title, text, callback, options = {}) {
        Swal.fire({
            icon: 'warning',
            title,
            text,
            input: 'textarea',
            inputLabel: options.inputLabel || 'Motivo',
            inputPlaceholder: options.inputPlaceholder || 'Ingrese el motivo del cambio de estado',
            inputValidator: (value) => {
                if (!value?.trim()) {
                    return 'El motivo es requerido';
                }
                return null;
            },
            showCancelButton: true,
            confirmButtonColor: '#198754',
            cancelButtonColor: '#dc3545',
            confirmButtonText: options.confirmButtonText || 'Confirmar',
            cancelButtonText: options.cancelButtonText || 'Cancelar'
        }).then(result => {
            if (result.isConfirmed && callback) {
                callback(result.value);
            }
        });
    }

    successWithTimer(title, text, timer = 2500, callback) {
        Swal.fire({
            icon: 'success',
            title,
            text,
            timer: timer,
            showConfirmButton: false,
            timerProgressBar: true
        }).then(() => {
            if (callback) callback();
        });
    }
}

// Exportar como singleton
window.alertService = new AlertService();