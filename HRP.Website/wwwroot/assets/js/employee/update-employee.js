var updateEmployee = (function ($) {
    var $form;

    function registerEventSubmitForm() {
        $form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'Employee/Update/PersonalDetails',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: onUpdateSuccess,
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($form);
            }
        });
    }

    function onUpdateSuccess(response) {
        if (response.isSuccessful) {
            commonFunctions.showSuccessMessage($form.find('.msg-success'));
        } else {
            if (response.errors) {
                $form.validate().showErrors(response.errors);
                commonFunctions.showValidationSummary($form);
            } else {
                commonFunctions.showErrorMessage($form.find('.msg-error'), response.message);
            }
        }
    }

    function initModule() {
        $form = $('#personalDetailsForm');
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
        initModule: initModule
    };
}(jQuery));

$(function () {
    updateEmployee.initModule();
});