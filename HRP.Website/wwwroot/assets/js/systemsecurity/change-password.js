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