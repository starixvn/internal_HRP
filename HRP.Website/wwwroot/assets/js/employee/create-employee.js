var createEmployee = (function ($) {
    var $createPersonalDetalsForm, $personalDetailsStep, $employmentStep;

    function registerEventSubmitForm() {
        $createPersonalDetalsForm.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'EmployeeManagement/Create/PersonalDetails',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: onUpdateSuccess,
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($createPersonalDetalsForm);
            }
        });
    }

    function onUpdateSuccess(response) {
        if (response.isSuccessful) {
            $personalDetailsStep.removeClass('active').addClass('finished');
            $employmentStep.addClass('active');
            $('#EmploymentHeaderTab').trigger('click');
        } else {
            $createPersonalDetalsForm.validate().showErrors(response.errors);
            commonFunctions.showValidationSummary($createPersonalDetalsForm);
        }
    }

    function initModule() {
        $createPersonalDetalsForm = $('#personalDetailsForm');
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');

        commonFunctions.initDatetimepicker($createPersonalDetalsForm);
        employeeCommonFuncs.initForm($createPersonalDetalsForm);
        registerEventSubmitForm();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployee.initModule();
});