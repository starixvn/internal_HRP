var commonFunctions = (function ($) {
    var rootURL;

    function showErrorMessage(message) {
        $('.msg-error').text(message).removeClass('hidden');

        setTimeout(function () {
            $('.msg-error').text('').addClass('hidden');
        }, 5000);
    }

    function showSuccessMessage(message) {
        $('.msg-success').text(message).removeClass('hidden');

        setTimeout(function () {
            $('.msg-success').text('').addClass('hidden');
        }, 5000);
    }

    function handleAjaxError(xhr, textStatus, errorThrown) {
        var url;

        switch (xhr.status) {
            case 401:
                url = 'auth/login';
                break;

            case 405:
                url = 'error/405';
                break;

            case 500:
                url = 'error/500';
                break;
        }

        window.location = rootURL + url;
    }

    function initModule() {
        rootURL = $('#RootURL').val();
    }

    return {
        initModule: initModule,
        rootURL: $('#RootURL').val(),
        handleAjaxError: handleAjaxError,
        showErrorMessage: showErrorMessage,
        showSuccessMessage: showSuccessMessage
    }
}(jQuery));

$(document).ready(function () {
    commonFunctions.initModule();
});