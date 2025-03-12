import '../lib/toastr.js/toastr.min.js';
var $ = jQuery.noConflict();

$(function () {
    var successMessage = $('#toastr-success-message').data('message');
    if (successMessage) {
        toastr.success(successMessage);
    }

    var errorMessage = $('#toastr-error-message').data('message');
    if (errorMessage) {
        toastr.error(errorMessage);
    }

    var warningMessage = $('#toastr-warning-message').data('message');
    if (warningMessage) {
        toastr.warning(warningMessage);
    }

    var infoMessage = $('#toastr-info-message').data('message');
    if (infoMessage) {
        toastr.info(infoMessage);
    }
});

// Check if the 'toastr-success-message' flag exists in sessionStorage
document.addEventListener('DOMContentLoaded', function () {
    const message = sessionStorage.getItem('toastr-success-message');

    if (message) {
        // Show the notification
        toastr.success(message);

        // Remove the flag from sessionStorage
        sessionStorage.removeItem('toastr-success-message');
    }
});
