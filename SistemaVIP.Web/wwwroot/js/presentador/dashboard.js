// dashboard.js
$(document).ready(function () {
    // Inicializar componentes
    inicializarSidebar();
    inicializarTabs();
    actualizarResumen();

    // Evento para el botón de toggle del sidebar
    $('#sidebarCollapse').on('click', toggleSidebar);

    // Evento para actualizar resumen al cambiar de tab
    $('#presentadorTabs button').on('shown.bs.tab', function (e) {
        actualizarResumen();
    });

    // Evento para el botón de toggle del sidebar
    $('#sidebarCollapse').on('click', toggleSidebar);

    // Evento para actualizar resumen al cambiar de tab
    $('#presentadorTabs button').on('shown.bs.tab', function (e) {
        actualizarResumen();
    });

    // Agregar evento para colapsar submenues al hacer clic fuera de ellos
    $(document).on('click', function (event) {
        const $trigger = $(".sidebar-header, .list-unstyled");
        if ($trigger !== event.target && !$trigger.has(event.target).length) {
            $('.collapse').collapse('hide');
        }
    });
    $(document).on('click', function (event) {
        const $trigger = $(".sidebar-header, .list-unstyled");
        if ($trigger !== event.target && !$trigger.has(event.target).length) {
            $('.collapse').collapse('hide');
        }
    });

});

function inicializarSidebar() {
    // Verificar estado inicial del sidebar desde localStorage
    const sidebarState = localStorage.getItem('sidebarState');
    if (sidebarState === 'collapsed') {
        $('#sidebar').addClass('active');
        $('#content').addClass('active');
    }
}

function toggleSidebar() {
    const sidebar = $('#sidebar');
    const content = $('#content');

    // Toggle las clases
    sidebar.toggleClass('active');
    content.toggleClass('active');

    // Guardar estado en localStorage
    localStorage.setItem('sidebarState',
        sidebar.hasClass('active') ? 'collapsed' : 'expanded'
    );
}

function inicializarTabs() {
    // Manejar cambio de tabs
    $('#presentadorTabs button').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    // Mantener tab activo después de recargar
    let activeTab = localStorage.getItem('activeTab');
    if (activeTab) {
        $(`#presentadorTabs button[data-bs-target="${activeTab}"]`).tab('show');
    }

    // Guardar tab activo
    $('#presentadorTabs button').on('shown.bs.tab', function (e) {
        localStorage.setItem('activeTab', $(e.target).data('bs-target'));
    });
}

function actualizarResumen() {
    // Actualizar contadores del sidebar
    $.ajax({
        url: '/Presentador/ObtenerResumenSemanal',
        type: 'GET',
        success: function (response) {
            if (response.success) {
                actualizarContadores(response.data);
            }
        }
    });
}

function actualizarContadores(data) {
    // Actualizar contadores en el sidebar
    $('#total-servicios').text(data.cantidadServicios || 0);
    $('#total-horas').text(data.totalHoras || 0);
    $('#total-ganancias').text(`$${(data.totalGanancias || 0).toLocaleString('es-MX')}`);

    // Actualizar contadores de estados
    $('#count-pendientes').text(data.pendientes || 0);
    $('#count-proceso').text(data.enProceso || 0);
    $('#count-confirmar').text(data.porConfirmar || 0);
    $('#count-finalizados').text(data.finalizados || 0);
}