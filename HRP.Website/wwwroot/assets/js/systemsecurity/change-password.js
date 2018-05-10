var changePassword = (function ($) {
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

    commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
});