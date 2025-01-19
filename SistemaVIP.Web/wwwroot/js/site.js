$(document).ready(function () {
    // Toggle Sidebar
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar, #content').toggleClass('active');
    });

    // Active Link
    const currentUrl = window.location.pathname;
    $('#sidebar ul.components a').each(function () {
        if ($(this).attr('href') === currentUrl) {
            $(this).addClass('active');
            $(this).parents('.collapse').addClass('show');
        }
    });

    // Initialize Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });
});