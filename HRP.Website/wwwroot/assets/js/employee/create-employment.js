'use strict';

var createEmployment = (function ($) {
    var $createEmploymentForm;

    function initModule() {
        $createEmploymentForm = $('#createEmploymentForm');
        employmentCommonFuncs.initForm($createEmploymentForm);
    }

    return {
        initModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                $createEmploymentForm.find('.list-message-error').addClass('hidden');
            } else {
                if (response.errors) {
                    $createEmploymentForm.validate().showErrors(response.errors);
                } else {
                    $createEmploymentForm.find('.list-message-error').removeClass('hidden')
                        .append('<span class="message">' + response.message + '</span>');
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.initModule();
});