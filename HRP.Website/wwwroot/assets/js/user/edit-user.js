var editUser = (function ($) {
    function displaySuccessMessage() {
        var alert = $('#edit-user-alert');

        alert.removeClass('hidden');

        window.setTimeout(function () {
            alert.addClass('hidden');
        }, 2000);
    }

    function displayValidationErrorMessages(errors) {
        errors.forEach(function (error) {
            $('[data-valmsg-for="' + error.key + '"]').text(error.errorMessage);
        });
    }

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

$(document).ready(function () {
    if (!$('#CanEdit').prop('checked')) {
        $('input[type="text"]').attr('readonly', 'readonly');
        $('input[type="checkbox"]').attr('disabled', 'disabled');
        $('form').submit(false);
    }

    commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
});