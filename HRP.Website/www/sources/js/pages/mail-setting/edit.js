import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';

$(document).ready(function () {
    var targetNode = document.getElementById('validation-summary');

    if ($('.field-validation-error', targetNode).length > 0) {
        $(targetNode).removeClass('hidden');
    } else {
        $(targetNode).addClass('hidden');
    }

    if($('.msg-success')){
        setTimeout(function () {
            $('.msg-success').addClass('hidden');
        }, 3000);
    }

    if($('.msg-error')){
        setTimeout(function () {
            $('.msg-error').addClass('hidden');
        }, 3000);
    }

    commonFunctions.showHideValidationSummary(document.getElementById('validation-summary'));
});