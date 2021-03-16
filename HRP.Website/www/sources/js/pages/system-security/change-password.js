import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';

window.changePassword = (function ($) {
    return {
        onChangeSuccess: function (response) {
            var $form = $('#changePasswordForm');
            if (response.isSuccessful) {
                commonFunctions.showSuccessMessage($form.find('.msg-success'));
            } else {
                if (response.errors) {
                    $form.validate().showErrors(response.errors);
                } else {
                    commonFunctions.showErrorMessage($form.find('.msg-error'), response.message);
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    var targetNode = document.getElementById('validation-summary');

    if ($('.field-validation-error', targetNode).length > 0) {
        $(targetNode).removeClass('hidden');
    } else {
        $(targetNode).addClass('hidden');
    }

    $('#ShowPassword').on('click', function () {
        if ($(this).prop('checked')) {
            $('input[type=password]').attr('type', 'text');
        } else {
            $('input[type=text]').attr('type', 'password');
        }
    });

    $('.js-cancel').on('click', function () {
        $('#ShowPassword').prop('checked', false);
        $('input[type=text]').attr('type', 'password');

        $('input[type=password]').val('');
    });

    commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
});