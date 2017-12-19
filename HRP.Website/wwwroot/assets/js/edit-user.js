var editUser = (function ($) {
    var displaySuccessMessage = function () {
        var alert = $('#edit-user-alert');

        alert.removeClass('hidden');

        window.setTimeout(function () {
            alert.addClass('hidden');
        }, 2000);
    };

    var displayValidationErrorMessages = function (errors) {
        errors.forEach(function (error) {
            $('[data-valmsg-for="' + error.key + '"]').text(error.errorMessage);
        });
    };

    return {
        onEditBegin: function () {
            $('#btn-edit').button('loading');
        },
        onEditSuccess: function (response, status) {
            if (response.isSuccessful) {
                displaySuccessMessage();
            } else {
                if (response.errors) {
                    displayValidationErrorMessages(response.errors);
                }

                if (response.message) {
                    $('#edit-user-error-alert').removeClass('hidden').find('span').text(response.message);
                }
            }
        },
        onEditComplete: function () {
            $('#btn-edit').button('reset');
        }
    };
}(jQuery));