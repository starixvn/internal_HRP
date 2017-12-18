var createUser = (function ($) {
    $(function () {
        var settings = $.data($('#create-user-form')[0], 'validator').settings;
        settings.onkeyup = false;
    });

    var displayValidationErrorMessages = function (errors) {
        errors.forEach(function (error) {
            $('[data-valmsg-for="' + error.key + '"]').text(error.errorMessage);
        });
    };

    var displaySuccessMessage = function (username, redirectTo) {
        var alert = $('#create-user-alert');
        var delay = 5;
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
            $('#btn-create').button('loading')
        },
        onCreateSuccess: function (response, status) {
            if (response.isSuccessful) {
                displaySuccessMessage(response.username, response.redirectTo);
            } else {
                if (response.errors) {
                    displayValidationErrorMessages(response.errors);
                }

                if (reponse.message) {
                    $('#create-user-error-alert').removeClass('hidden').text(response.message);
                }
            }
        },
        onCreateComplete: function () {
            $('#btn-create').button('reset');
        }
    };
}(jQuery));