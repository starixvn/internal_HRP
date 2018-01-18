var updateEmployee = (function ($) {
    var $form, $btnSubmit;

    function registerEventSubmitForm() {
        $btnSubmit.on('click', function () {
            if ($form.valid()) {
                $form.submit();
            } else {
                commonFunctions.showValidationSummary($form);
            }
        });
    }

    function initModule() {
        $form = $('#personalDetailsForm');
        $btnSubmit = $form.find('.js-btnSubmit');
        commonFunctions.initDatetimepicker($form);

        if ($form.find('#IsHost').prop('checked')) {
            employeeCommonFuncs.initForm($form);
            registerEventSubmitForm();
        } else {
            $form.find('input[type="text"]').attr('readonly', 'readonly');
            $form.find('select').attr('disabled', 'disabled');
            $form.submit(false);
            $form.find('.img-container').css('cursor', 'default');
        }
    }

    return {
        initModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                commonFunctions.showSuccessMessage('Updated sucessfully!');
            } else {
                if (response.errors) {
                    $form.validate().showErrors(response.errors);
                } else {
                    $form.find('.list-message-error')
                        .append('<span class="message">' + response.message + '</span>');
                }

                commonFunctions.showValidationSummary($form);
            }
        }
    };
}(jQuery));

$(function () {
    updateEmployee.initModule();
});