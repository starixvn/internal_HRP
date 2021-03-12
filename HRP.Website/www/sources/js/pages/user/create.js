import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';
import 'bootstrap/dist/js/bootstrap';

global.createUser = (function ($) {
    function displayValidationErrorMessages(errors) {
        errors.forEach(function (error) {
            $('[data-valmsg-for="' + error.key + '"]').text(error.errorMessage);
        });
    }

    function displaySuccessMessage(username, redirectTo) {
        var alert = $('#create-user-alert');
        var delay = 3;
        var textAlert = username + ' account has been created successfully. You will be redirected in ';
        alert.removeClass('hidden');
        $('#alert-message', alert).text(textAlert + delay + ' seconds.');

        window.setInterval(function () {
            if (delay === 0) {
                window.location.href = redirectTo;
            } else {
                delay--;
                $('#alert-message', alert).text(textAlert + delay + ' seconds.');
            }
        }, 1000);
    }

    return {
        onCreateBegin: function () {
            $('#btn-create').button('loading');
        },
        onCreateSuccess: function (response, status) {
            if (response.isSuccessful) {
                displaySuccessMessage(response.username, response.redirectTo);
            } else {
                if (response.errors) {
                    displayValidationErrorMessages(response.errors);
                }

                if (response.message) {
                    $('#create-user-error-alert').removeClass('hidden').text(response.message);
                }
            }
        },
        onCreateComplete: function () {
            $('#btn-create').button('reset');
        }
    };
}(jQuery));

$(document).ready(function () {
    var $form = $('#create-user-form')[0];

    $('#show-password').on('change', function () {
        if ($(this).is(':checked')) {
            $('#Password').attr('type', 'text');
        } else {
            $('#Password').attr('type', 'password');
        }
    });

    $form.reset();

    commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
});